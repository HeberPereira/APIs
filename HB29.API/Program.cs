using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

namespace hb29.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Start application API...");
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Host terminated unexpectedly");
                Console.WriteLine(ex.ToString());
                Log.Fatal(ex, "Host terminated unexpectedly");
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
                    
                    Console.WriteLine("Enviroment..." + envName);

                    hostConfig.SetBasePath(Directory.GetCurrentDirectory());
                    hostConfig.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                    //config environment path
                    if (!string.IsNullOrWhiteSpace(envName))
                        hostConfig.AddJsonFile($"appsettings.{envName}.json", optional: true);
                    hostConfig.AddEnvironmentVariables();
                    var settings = hostConfig.Build();

                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseSerilog();
    }
}
