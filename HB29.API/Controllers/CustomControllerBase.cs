using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using hb29.API.Filter;
using hb29.API.Helpers;
using hb29.API.Helpers.PaginationUri;
using hb29.API.Repository;
using hb29.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace hb29.API.Controllers
{
    public class CustomControllerBase : ControllerBase
    {
        public IConfiguration _configuration;
        public readonly DefaultContext _context;
        public readonly AzureQueueStorage _queueService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public List<string> ListGroupByCurrentUser
        {
            get { return GetUserGroups().GetAwaiter().GetResult(); }
        }

        public CustomControllerBase(IConfiguration configuration, DefaultContext context, AzureQueueStorage queueService, ILogger logger)
        {
            _configuration = configuration;
            _queueService = queueService;
            _context = context;
            _logger = logger;
            _context.SetQueueService(_queueService);            
        }


        protected Helpers.BusinessValidation BusinessValidation { get; set; } = new();

        protected new IActionResult Ok(object value = null)
        {
            return Ok(value, "Action executed sucessfully.");
        }

        protected IActionResult Ok<T>(List<T> item, PaginationFilter filter, int totalRecords, IUriPagination uriPagination) 
        {
            var route = Request.Path.Value;
            
            var pagedResponse = PaginationHelper.CreatePagedResponse<T>(item, filter, totalRecords, uriPagination, route);

            return Ok(pagedResponse, "Action executed sucessfully.");
        }

        protected async Task CheckDups<T>(Repository.DefaultContext context, System.Linq.Expressions.Expression<Func<T, bool>> condition, string errorMessage) where T : Models.ModelBase
        {
            //customer is unique in country
            var checkDups = context.Set<T>().Where(condition);

            if (await checkDups.AnyAsync())
            {
                var first = await checkDups.FirstAsync();

                this.BusinessValidation.AddError(
                    errorMessage
                    + (first.DeletedAt != null ? $" (item deleted at {first.DeletedAt})" : "")
                );
            }
        }

        protected string GetUser()
        {
            return HttpContext.User.FindFirstValue(ClaimTypes.Upn);
        }

        protected async Task<List<string>> GetUserGroups(string upn = null)
        {
            try
            {
                if (upn == null)
                    upn = this.GetUser();

                var graphHelper = new Helpers.GraphHelper(_configuration);
                
                return await graphHelper.GetUserGroups(upn);
            }
            catch (ServiceException ex)
            {
                _logger.LogInformation(ex.Message);
                return new List<string>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new List<string>();
            }
        }

        protected async Task<List<User>> GetUsersOfGroup(string adGroupId)
        {
            try
            {
                var graphHelper = new Helpers.GraphHelper(_configuration);

                return await graphHelper.GetUsersOfGroupAsync(adGroupId);
            }
            catch(ServiceException ex)
            {
                _logger.LogInformation(ex.Message);
                return new List<User>();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return new List<User>();
            }
        }

        protected IActionResult Ok(object value, string message)
        {
            return base.Ok(new
            {
                success = true,
                data = value,
                message = message
            });
        }

        protected IActionResult InternalServerError(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            //TODO: removido o stack track por colocar muitas informações desnecessarias no resultado das mensagens.
            return StatusCode(
                Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError,
                $"{ex.Message} "
            );
        }

        protected IActionResult NoDataFound(string message)
        {
            return StatusCode(
                Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound,
               message
            );
        }

        /// <summary>
        /// Validates model state and add business error message if any.
        /// </summary>
        protected void ValidateModelState<T>(T item)
        {
            if (!TryValidateModel(item, nameof(T)))
                foreach (string message in ModelState.SelectMany(m => m.Value.Errors).Select(e => e.ErrorMessage))
                    this.BusinessValidation.AddError(message);
        }

        protected IActionResult UnprocessableError(string ex)
        {
            return StatusCode(
                Microsoft.AspNetCore.Http.StatusCodes.Status422UnprocessableEntity,
                $"{ex}"
            );
        }

        

        /// <summary>
        /// Soft deletes entities listed in IDs list, by updating DeletedAt and DeletedBy property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        protected async Task<bool> SoftRemove<T>(Repository.DefaultContext context, ICollection<long> ids) where T : Models.ModelBase
        {
            var items = await context.Set<T>()
                   .Where(i => ids.Contains(i.Id))
                   .ToListAsync();

            foreach (T i in items)
            {
                i.DeletedAt = DateTime.UtcNow;
                i.DeletedBy = GetUser();
            }

            return true;
        }

        protected async Task<bool> HardRemove<T>(Repository.DefaultContext context, ICollection<long> ids) where T : Models.ModelBase
        {
            await context.Set<T>()
                        .Where(i => ids.Contains(i.Id))
                        .ForEachAsync(i =>
                        {
                            i.DeletedAt = DateTime.UtcNow;
                            context.Remove(i);
                        });

            return true;
        }
    }
}