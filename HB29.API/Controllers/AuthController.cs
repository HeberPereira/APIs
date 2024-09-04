using AutoMapper;
using hb29.API.DTOs;
using hb29.API.Models;
using hb29.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using hb29.API.Enums;

namespace hb29.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [Authorize]
    public class AuthController : CustomControllerBase
    {
        private readonly ILogger<AuthController> _logger;        
        private readonly IMapper _mapper;

        public AuthController(Repository.DefaultContext context, ILogger<AuthController> logger, IMapper mapper,
            IConfiguration configuration, AzureQueueStorage queueService) : base(configuration, context, queueService, logger)
        {
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("permissions")]
        [ProducesResponseType(typeof(IList<string>), (int)HttpStatusCode.OK)]
        public IActionResult Get()
        {
            try
            {
                var claisMemoryCache = Startup.ClaimsMemoryCache;
                string upn = HttpContext.User.FindFirstValue(ClaimTypes.Upn);

                List<Models.Profile> profiles = new();
                List<string> permissions = new();

                try
                {
                    if (!string.IsNullOrEmpty(upn))
                    {
                        profiles = claisMemoryCache.Get(upn);
                        _logger.LogDebug($"Profiles of {upn} ({profiles.Count}): " + string.Join("\n- ", profiles));

                        permissions = profiles.SelectMany(profile => profile.Permissions).Select(p => p.Name).ToList();
                        _logger.LogDebug($"Permissions of {upn} ({permissions.Count}): " + string.Join("\n- ", permissions));
                    }
                    else
                    {
                        _logger.LogWarning("UPN not found in request context.");
                        return BadRequest("UPN not found in request context.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex.Message);
                    return InternalServerError(ex);
                }

                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete("permissions")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public IActionResult Delete(string upn)
        {
            try
            {
                var claisMemoryCache = Startup.ClaimsMemoryCache;
                upn ??= HttpContext.User.FindFirstValue(ClaimTypes.Upn);

                claisMemoryCache.Remove(upn);
                return Ok($"Permission cache removed for '{upn}'.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet("get-accept-user")]
        [ProducesResponseType(typeof(PrivacyPolicyDTO), (int)HttpStatusCode.OK)]
        public IActionResult GetExistTermsToUser()
        {
            try
            {
                PrivacyPolicyDTO policyAccept = new PrivacyPolicyDTO();
                var user = HttpContext.User.FindFirstValue(ClaimTypes.Upn);
                var privacyPolicyCurrent = _context.PrivacyPolicies.FirstOrDefault(x => x.PrivacyPolicyStatus == PrivacyPolicyStatusEnum.Current);

                if (privacyPolicyCurrent != null)
                {
                    if (!_context.UserTerms.Any(i => i.DeletedAt == null && i.UserName == user && i.PrivacyPolicyId.Equals(privacyPolicyCurrent.Id)))
                    {
                        policyAccept = _mapper.Map<PrivacyPolicyDTO>(privacyPolicyCurrent);
                    }
                }

                return Ok(policyAccept);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost("accept-term")]
        [ProducesResponseType(typeof(UserTerm), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Save()
        {
            try
            {
                var user = HttpContext.User.FindFirstValue(ClaimTypes.Upn);
                var privacyPolicyCurrent = _context.PrivacyPolicies.FirstOrDefault(x => x.PrivacyPolicyStatus == PrivacyPolicyStatusEnum.Current);

                var toAdd = new UserTerm() { UserName = user, PrivacyPolicyId = privacyPolicyCurrent.Id };
                _context.Entry<UserTerm>(toAdd).CurrentValues.SetValues(toAdd);
                toAdd.CreatedAt = DateTime.UtcNow;
                toAdd.CreatedBy = GetUser();
                _context.UserTerms.Add(toAdd);
                await _context.SaveChangesAsync();

                return Ok(toAdd);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet("profiles-groups-ad")]
        [ProducesResponseType(typeof(IList<string>), (int)HttpStatusCode.OK)]
        public IActionResult GetGroupsAd()
        {
            try
            {
                var claisMemoryCache = Startup.ClaimsMemoryCache;
                string upn = HttpContext.User.FindFirstValue(ClaimTypes.Upn);

                List<Models.Profile> profiles = new();

                if (!string.IsNullOrEmpty(upn))
                {
                    profiles = claisMemoryCache.Get(upn);
                    _logger.LogDebug($"Profiles of {upn} ({profiles.Count}): " + string.Join("\n- ", profiles));
                    return Ok(profiles.Select(s => new { Id = s.AdGroupId }));
                }
                else
                {
                    _logger.LogWarning("UPN not found in request context.");
                    return BadRequest("UPN not found in request context.");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}