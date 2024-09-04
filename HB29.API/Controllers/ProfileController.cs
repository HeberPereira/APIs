using AutoMapper;
using hb29.API.Controllers.CustomRoles;
using hb29.API.DTOs;
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
    [Route("api/profile")]
    [ApiController]
    public class ProfileController : CustomControllerBase
    {
        private readonly IMapper _mapper;

        public ProfileController(Repository.DefaultContext context, IMapper mapper, IConfiguration configuration, AzureQueueStorage queueService,
            ILogger<ProfileController> logger) : base(configuration, context, queueService, logger)
        {
            _mapper = mapper;
        }

        [HttpGet]
        //[ProducesResponseType(typeof(IList<Models.Profile>), (int)HttpStatusCode.OK)]
        [Produces("application/json")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var profiles = await _context.Profiles.Include(x => x.Permissions)
                    .Where(x => x.DeletedAt == null)
                    .ToListAsync();

                var result = _mapper.Map<List<ProfileDTO>>(profiles);
                
                Response.StatusCode = (int)HttpStatusCode.OK;

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Verifies if new or modified entity complies with business rules.
        /// </summary>
        [NonAction]
        protected bool Validate(Models.Profile item)
        {
            ValidateModelState<Models.Profile>(item);

            if (string.IsNullOrEmpty(item.Name) || string.IsNullOrWhiteSpace(item.Name))
                this.BusinessValidation.AddError("Name is required");

            return this.BusinessValidation.IsValid;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProfileDTO), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Save([FromBody] ProfileDTO data)
        {
            try
            {
                var toAdd = new Models.Profile();
                _context.Entry<Models.Profile>(toAdd).CurrentValues.SetValues(data.ToSetValuesModel());

                var toAddPermissions = await _context.Permissions
                    .Where(t => data.Permissions.Select(tt => tt.Id).Contains(t.Id))
                    .ToListAsync();

                foreach (var item in toAddPermissions)
                    toAdd.Permissions.Add(item);

                toAdd.CreatedAt = DateTime.UtcNow;
                toAdd.CreatedBy = GetUser();

                if (!Validate(toAdd))
                    return new UnprocessableEntityObjectResult(this.BusinessValidation);

                _context.Profiles.Add(toAdd);
                await _context.SaveChangesAsync();

                var result = _mapper.Map<ProfileDTO>(data);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(ProfileDTO), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Update([FromRoute] long id, [FromBody] ProfileDTO data)
        {
            try
            {
                data.Id = id;
                var existingItem = await _context.Profiles.Include(x => x.Permissions)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (existingItem == null)
                    return NotFound();

                _context.Entry<Models.Profile>(existingItem).CurrentValues.SetValues(data);

                var existingPermissions = existingItem.Permissions
                    .ToList();

                //remove permissions
                var removedPermissionIds = existingPermissions.Select(t => t.Id)
                    .Except(data.Permissions.Select(tt => tt.Id));

                foreach (Permission toRemove in existingPermissions.Where(t => removedPermissionIds.Contains(t.Id)))
                    existingItem.Permissions.Remove(toRemove);

                //add new permissions
                var addedPermissionIds = data.Permissions.Select(tt => tt.Id)
                    .Except(existingPermissions.Select(t => t.Id));

                var addedPermissions = await _context.Permissions
                    .Where(t => addedPermissionIds.Contains(t.Id))
                    .ToListAsync();

                foreach (var item in addedPermissions)
                    existingItem.Permissions.Add(item);

                existingItem.UpdatedAt = DateTime.UtcNow;
                existingItem.UpdatedBy = GetUser();

                if (!Validate(existingItem))
                    return new UnprocessableEntityObjectResult(this.BusinessValidation);

                _context.Profiles.Update(existingItem);
                await _context.SaveChangesAsync();

                var result = _mapper.Map<ProfileDTO>(existingItem);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Remove([FromBody] List<long> ids)
        {
            try
            {
                await base.SoftRemove<Models.Profile>(_context, ids);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }



        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var retorno = await _context.Profiles.Include(x => x.Permissions)
                                                    .FirstOrDefaultAsync(x => x.Id == id);

                var result = _mapper.Map<ProfileDTO>(retorno);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}