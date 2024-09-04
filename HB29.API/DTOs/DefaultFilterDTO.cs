using System.Collections.Generic;

namespace hb29.API.DTOs
{
    public class DefaultFilterDTO
    {
        public List<long> Ids { get; set; } = new();
        public long? ExcludedId { get; set; }
        public long? ClusterId { get; set; }
        public long? CountryId { get; set; }
        public long? CustomerId { get; set; }
        public long? RegionId { get; set; }
        public List<long> CustomerIds { get; set; } = new();
        public long? ActivityTypeId { get; set; }
        public long? TechnologyId { get; set; }
        public List<long> TechnologyIds { get; set; } = new();
        public long? NodeTypeId { get; set; }
        public List<long> NodeTypeIds { get; set; } = new();
        public List<string> NodeNames { get; set; } = new();
        public List<long> NodeTemplateIds { get; set; } = new();
        public string ActivityCode { get; set; }
    }
}
