using hb29.API.Models;
using hb29.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace hb29.API.Controllers
{
    [Authorize]
    [Route("api/audit-log")]
    [ApiController]
    public class AuditLogController : CustomControllerBase
    {
        public AuditLogController(Repository.DefaultContext context,
            IConfiguration configuration, AzureQueueStorage queueService, ILogger<AuditLogController> logger) : base(configuration, context, queueService, logger)
        {
            
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<AuditLog>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data = await _context.AuditLogs
                    .ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id}/{table}")]
        public async Task<IActionResult> GetById(long id, string table)
        {
            try
            {
                var data = await _context.AuditLogs
                   .Where(c => c.Id == id && c.TableName.Equals(table))
                   .ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}