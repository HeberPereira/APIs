using AutoMapper;
using hb29.API.Helpers;
using hb29.API.Helpers.PaginationUri;
using hb29.API.Repository;
using hb29.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hb29.API.Filter;

namespace hb29.API.Controllers
{
    public abstract class PagedController<TEntity, TResult, TFilter> : CustomControllerBase
        where TEntity : class
        where TResult : class
        where TFilter : IQueryFilter
    {
        internal readonly DefaultContext context;
        internal readonly IMapper _mapper;
        internal readonly IUriPagination uriPagination;
        
        public PagedController(DefaultContext context, IMapper mapper, IUriPagination uriPagination, IConfiguration configuration, AzureQueueStorage queueService,
            ILogger<PagedController<TEntity,TResult,TFilter>>logger) : base(configuration, context, queueService, logger)
        {
            this.context = context;
            this._mapper = mapper;
            this.uriPagination = uriPagination;
        }

        protected abstract IQueryable<TEntity> BuildQueryBase(TFilter queryFilter);

        protected async Task<PagedResponse<List<TResult>>> BuildPagination(TFilter queryFilter, IQueryable<TEntity> query)
        {
            var validFilter = new PaginationFilter(queryFilter.Filter.PageNumber, queryFilter.Filter.PageSize);
            var route = Request.Path.Value;

            var totalRecords = query.Count();
            var result = await query
                  .Skip((validFilter.PageNumber - 1) * queryFilter.Filter.PageSize)
                  .Take(queryFilter.Filter.PageSize)
                  .ToListAsync();
            var pagedData = _mapper.Map<List<TResult>>(result);

            var pagedResponse = PaginationHelper.CreatePagedResponse<TResult>(pagedData, validFilter, totalRecords, uriPagination, route);
            return pagedResponse;
        }
    }
}