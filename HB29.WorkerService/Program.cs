using Azure.Identity;
using hb29.Shared.Services;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Microsoft.AspNetCore.Hosting;

namespace hb29.WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Start Console Process Queue...");
                Log.Information("Starting worker host");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Host terminated unexpectedly");
                Console.WriteLine(ex.ToString());
                Log.Fatal(ex, "Worker terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(hostConfig =>
                 {
                     var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                     hostConfig.SetBasePath(Directory.GetCurrentDirectory());
                     hostConfig.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                     //config environment path
                     if (!string.IsNullOrWhiteSpace(envName))
                         hostConfig.AddJsonFile($"appsettings.{envName}.json", optional: true);

                     var settings = hostConfig.Build();

                     //config Azure Key Vault
                     var keyVaultEndpoint = settings["AzureKeyVaultEndpoint"];
                     Console.WriteLine($"Connecting Azure Key Vault {keyVaultEndpoint}.");
                     var defaultCredentials = new DefaultAzureCredential();

                     hostConfig.AddAzureKeyVault(new Uri(keyVaultEndpoint), defaultCredentials,
                                     new AzureKeyVaultConfigurationOptions
                                     {
                                         ReloadInterval = TimeSpan.FromMinutes(5)
                                     });

                     hostConfig.AddEnvironmentVariables();
                     Console.WriteLine($"Connected Azure Key Vault {keyVaultEndpoint}.");
                 })
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    //Config applicationInsights
                    var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
                    telemetryConfiguration.ConnectionString = configuration["ApplicationInsights-ConnectionString"];

                    //Config Logger
                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .ReadFrom.Configuration(configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.Console(theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
                        .WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces)
                        .CreateLogger();

                    //SQL
                    services.AddDbContext<hb29.API.Repository.DefaultContext>(options =>
                        options.UseSqlServer(configuration["ConnectionStrings-DefaultConnection"])
                    );

                    //Azure Storage Services
                    //ContainerNameSettings containerOptions = configuration
                    //    .GetSection("ContainerNames")
                    //    .Get<ContainerNameSettings>();
                    //services.AddSingleton(containerOptions);
                    services.Configure<ContainerNameSettings>(configuration.GetSection("ContainerNames"));

                    //QueueNameSettings queueOptions = configuration
                    //    .GetSection("QueueNames")
                    //    .Get<QueueNameSettings>();
                    //services.AddSingleton(queueOptions);
                    services.Configure<QueueNameSettings>(configuration.GetSection("QueueNames"));

                    services.AddTransient<AzureQueueStorage>();
                    services.AddTransient<AzureBlobStorage>();
                    services.AddTransient<IStorageService, StorageService>();
                    services.AddTransient<AppNotificationService>();

                    //Background worker
                    services.AddHostedService<Worker>();

                    // App Insights registration
                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.AddSingleton<ITelemetryInitializer, ApplicationInsightsRoleNameInitialiser>();
                })
                .UseSerilog();

    }
}
