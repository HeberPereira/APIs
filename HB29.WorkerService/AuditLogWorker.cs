using hb29.API.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace hb29.WorkerService
{
    public class AuditLogWorker
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _logger;
        private readonly API.Helpers.BusinessValidation _validations;
        public AuditLogWorker(IServiceScopeFactory scopeFactory, ILogger logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _validations = new(_logger);
        }

        public async Task<API.Helpers.BusinessValidation> ExecuteAsync(AuditLog data)
        {
            _logger.LogInformation($"Starting save log...");

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<API.Repository.DefaultContext>();

                _context.AuditLogs.Add(data);

                await _context.SaveChangesAsync();
            }

            return _validations;
        }
    }
}