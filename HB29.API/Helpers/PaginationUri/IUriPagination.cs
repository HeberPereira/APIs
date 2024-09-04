using hb29.API.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hb29.API.Helpers.PaginationUri
{
    public interface IUriPagination
    {
        public Uri GetPageUri(PaginationFilter filter, string route);
    }
}
