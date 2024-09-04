using System;
using System.Collections.Generic;

namespace hb29.API.DTOs
{
    public class FilterParameters
    {
        public List<Guid> Ids { get; set; }
        public long? CustomerId { get; set; }
        public DateTime? StartDate { get; set; }

        public FilterParameters()
        {
            this.Ids = new List<Guid>();
        }
    }
    public class IntegrationFinishActivity
    {
        public bool Success { get; set; }
        public List<Integration> Data { get; set; }
        public string message { get; set; }
    }

    public class Integration
    {
        public Guid Id { get; set; }
        public DateTime? FinishedAt { get; set; }
        public string StatusName { get; set; }
    }
}
