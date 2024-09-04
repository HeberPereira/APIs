using Microsoft.AspNetCore.WebUtilities;
using hb29.API.Filter;
using System;

namespace hb29.API.Helpers.PaginationUri
{
    public class UriPagination : IUriPagination
    {
        private readonly string _baseUri;
        public UriPagination(string baseUri)
        {
            _baseUri = baseUri;
        }
        public Uri GetPageUri(PaginationFilter filter, string route)
        {
            var _enpointUri = new Uri(string.Concat(_baseUri, route));
            var modifiedUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "pageNumber", filter.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", filter.PageSize.ToString());
            return new Uri(modifiedUri);
        }
    }
}
