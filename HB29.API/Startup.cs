using AutoMapper;
using hb29.Shared.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Threading.Tasks;
using hb29.API.Domain;
using hb29.API.Interfaces;
using hb29.API.Services;
using hb29.API.Helpers;

namespace hb29.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly string MyAllowSpecificOrigins = "AnyOriginCORS";
        private readonly IConfiguration _configuration;
        private static readonly Repository.ClaimsMemoryCache _claimsMemoryCache = new();

        public static Repository.ClaimsMemoryCache ClaimsMemoryCache
        {
            get { return _claimsMemoryCache; }
        }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Config applicationInsights
            var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
            telemetryConfiguration.ConnectionString = _configuration["ApplicationInsights-ConnectionString"];

            //Config Logger
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
                .ReadFrom.Configuration(_configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
                .WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces)
                .CreateLogger();

            //add auth settings
            AddAuthenticationService(services);

            // Creating policies that wraps the authorization requirements
            services.AddAuthorization();

            // App Insights registration
            services.AddScoped(f => Log.Logger);
            services.AddApplicationInsightsTelemetry(_configuration);
            services.AddSingleton<ITelemetryInitializer, ApplicationInsightsRoleNameInitialiser>();
            services.AddHttpContextAccessor();

            services.AddOptions();

            //add azure storage services
            AddStorageService(services);

            Console.WriteLine("Configuring Transient");
            //domain classes
            services.AddTransient<ISendEmail, SendEmail>();
            services.AddTransient<QueueNameSettings>();
            services.AddTransient<Helpers.Cryptography>();
            services.AddTransient<ProfileDomain>();
            
            //AutoMapper
            services.AddAutoMapper(typeof(Startup));

            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins, builder =>
                {
                    builder
                        .WithOrigins(
                            "http://localhost:4401",
                            "https://localhost:4401",
                            "https://localhost:5001"
                        )
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders("Content-Disposition")
                    ;
                });
            });

            services.AddSqlite<Repository.DefaultContext>(_configuration["ConnectionStrings-DefaultConnection"]);

            //services.AddDbContext<Repository.DefaultContext>(options =>
            //    options.UseSqlServer(_configuration["ConnectionStrings-DefaultConnection"])
            //);

            services.AddControllers().AddJsonOptions(options =>
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
                );
            services.AddSwaggerGen(c => AddSwaggerGenConfig(c));

            services.AddMvcCore().AddApiExplorer();
        }

        private void AddStorageService(IServiceCollection services)
        {
            //Azure storage services
            services.Configure<ContainerNameSettings>(_configuration.GetSection("ContainerNames"));
            services.Configure<QueueNameSettings>(_configuration.GetSection("QueueNames"));
            services.AddTransient<AzureQueueStorage>();
            services.AddTransient<AzureBlobStorage>();
            services.AddTransient<AzureTableService>();

            services.AddTransient<IStorageService, StorageService>();
        }

        /// <summary>
        /// Configure authetication service to use JWT Tokens and other configurations.
        /// </summary>
        private void AddAuthenticationService(IServiceCollection services)
        {
            // Setting configuration JWT
            string issuer = _configuration.GetValue<string>("JwtTokenConfig:Issuer");
            string audience = _configuration.GetValue<string>("JwtTokenConfig:Audience");
           
            services
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Audience = audience;
                    options.Authority = _configuration["AzureAd:Instance"];
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async ctx => await EnhanceTokenClaims(ctx)
                    };
                });
        }

        /// <summary>
        /// Add permissions as role claims into access token based on User Profiles (User AD Groups).
        /// </summary>
        private async Task EnhanceTokenClaims(TokenValidatedContext ctx)
        {
            //get user groups from AD
            string upn = ctx.Principal.FindFirstValue(ClaimTypes.Upn);

            if (string.IsNullOrEmpty(upn))
            {
                Console.WriteLine("[WRN] No UPN found in token.");
                return;
            }

            List<Models.Profile> profile = await _claimsMemoryCache.GetOrCreate(
                upn,
                async (upn) => await GetProfilesFromContext(ctx, upn)
            );

            var permissionsFromDb = profile
                .SelectMany(profile => profile.Permissions)
                .Select(p => p.Name)
                .ToList();

            var claims = permissionsFromDb
                .Select(permission => new Claim(ClaimTypes.Role, permission))
                .ToList();



            ((ClaimsIdentity)ctx.Principal.Identity).AddClaims(claims);
        }

        /// <summary>
        /// Fetch user claims from database context using User Principal Name (UPN).
        /// </summary>
        private async Task<List<Models.Profile>> GetProfilesFromContext(TokenValidatedContext ctx, string upn)
        {
            var graphHelper = new Helpers.GraphHelper(_configuration);
            var userGroups = await graphHelper.GetGroupsAsync(upn);

            //Get EF context
            var db = ctx.HttpContext.RequestServices.GetRequiredService<Repository.DefaultContext>();

            //get permissions from database
            var profileFromDb = await db.Profiles
                .Include(p => p.Permissions)
                .Where(p => userGroups.Contains(p.AdGroupId))
                .Distinct()
                .ToListAsync();

            return profileFromDb;
        }
        /// <summary>
        /// Set swagger service configurations.
        /// </summary>
        private static void AddSwaggerGenConfig(SwaggerGenOptions c)
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "hb29.API", Version = "v1" });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", // must be lower case
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {securityScheme, Array.Empty<string>()}
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "hb29.API v1"));                
            }

            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

            //App Insights Logging
            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins); //must be between UserRouting() and UseEndpoints()

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
