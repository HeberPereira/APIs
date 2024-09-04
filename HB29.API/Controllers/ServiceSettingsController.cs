using AutoMapper;
using hb29.API.Controllers.CustomRoles;
using hb29.API.DTOs;
using hb29.API.Models;
using hb29.API.Repository;
using hb29.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace hb29.API.Controllers
{
    [Authorize]
    [Route("api/service-settings")]
    [ApiController]
    public class ServiceSettintsController : CustomControllerBase
    {
        private IMapper _mapper;

        public ServiceSettintsController(DefaultContext context, IMapper mapper, IConfiguration configuration, AzureQueueStorage queueService,
           ILogger<ServiceSettintsController> logger) : base(configuration, context, queueService, logger)
        {
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<ServiceSettingDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data = await _context.ServiceSettings
                    .ToListAsync();

                var result = _mapper.Map<List<ServiceSettingDTO>>(data);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(ServiceSettingEnum id)
        {
            try
            {
                var item = await _context.ServiceSettings.FirstOrDefaultAsync(x => x.Id == id);

                var result = _mapper.Map<ServiceSettingDTO>(item);

                if (item == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(List<ServiceSettingDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Update([FromBody] IList<ServiceSettingDTO> data)
        {
            List<ServiceSettingDTO> result = new List<ServiceSettingDTO>();

            try
            {
                foreach (var item in data)
                {
                    var existingItem = await _context.ServiceSettings
                    .FirstOrDefaultAsync(x => x.Id == item.Id);

                    if (existingItem == null)
                        return NotFound();

                    _context.Entry<ServiceSetting>(existingItem).CurrentValues.SetValues(item.ToSetValuesModel());

                    _context.ServiceSettings.Update(existingItem);
                    await _context.SaveChangesAsync();
                }

                var retorno = _mapper.Map<List<ServiceSettingDTO>>(result);

                return Ok(retorno);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}