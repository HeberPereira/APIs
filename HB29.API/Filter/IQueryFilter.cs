using hb29.API.Filter;

namespace hb29.API.Filter
{
    public interface IQueryFilter
    {
        PaginationFilter Filter { get; set; }
    }
}
