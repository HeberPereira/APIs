using Azure.Messaging.ServiceBus;
using hb29.Shared;
using hb29.Shared.Services;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace hb29.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly TelemetryClient _telemetryClient;
        private readonly AzureQueueStorage _queueService;
        private readonly QueueNameSettings _queueNameSettings;
        private readonly IStorageService _storageService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly AppNotificationService _appNotifications;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, TelemetryClient telemetryClient,
                      AzureQueueStorage queueService, IOptions<QueueNameSettings> queueNameSettings,
                      IStorageService storageService, IServiceScopeFactory scopeFactory,
                      AppNotificationService appNotifications, IConfiguration configuration)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
            _queueService = queueService;
            _queueNameSettings = queueNameSettings.Value;
            _storageService = storageService;
            _scopeFactory = scopeFactory;
            _appNotifications = appNotifications;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _queueService.ConfigureProcessorEvent(_queueNameSettings.AuditLogs, MessageHandlerAuditLog, ErrorHandler, stoppingToken);

            await Task.FromResult<bool>(true);
        }

        private async Task MessageHandlerAuditLog(ProcessMessageEventArgs args)
        {
            var message = args.Message;

            //Processar a mensagem recebida pela Fila
            await Task.CompletedTask;
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());

            return Task.CompletedTask;
        }
    }
}