using hb29.API.Filter;
using System;
using System.Collections.Generic;

namespace hb29.API.DTOs
{
    public class GenericFilter
    {
        public List<long> Ids { get; set; } = new();
        public long? ExcludedId { get; set; }                
        public long? ActivityTypeId { get; set; }
        public long? TechnologyId { get; set; }        
        public long? NodeTypeId { get; set; }        
        public List<string> NodeNames { get; set; } = new();
        public List<long> NodeTemplateIds { get; set; } = new();
        public long? CustomerId { get; set; }
        public long? CountryId { get; set; }
        public long? ClusterId { get; set; }
        public List<long> CustomerIds { get; set; } = new();
        public List<long> TechnologyIds { get; set; } = new();        
        public List<long> NodeSoftwareIds { get; set; } = new();
        public List<long> NodeTypeIds { get; set; } = new();
        public List<long> ScriptTypeIds { get; set; } = new();
        public string Name { get; set; }
        public PaginationFilter Filter { get; set; }
        public Guid? ConcurrencyToken { get; set; }
    }
}
