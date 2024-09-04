using AutoMapper;
using hb29.API.DTOs;
using hb29.Shared.Services;
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
    [Route("api/permission")]
    [ApiController]
    public class PermissionController : CustomControllerBase
    {
        private readonly IMapper _mapper;

        public PermissionController(Repository.DefaultContext context, IMapper mapper, IConfiguration configuration, AzureQueueStorage queueService,
           ILogger<PermissionController> logger) : base(configuration, context, queueService, logger)
        {
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<Models.Permission>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var retorno = await _context.Permissions
                    .Where(x => x.DeletedAt == null)
                    .ToListAsync();

                var result = _mapper.Map<List<PermissionDTO>>(retorno);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}