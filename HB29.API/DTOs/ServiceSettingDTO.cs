using hb29.API.Models;
using System;
using System.Collections.Generic;

namespace hb29.API.DTOs
{
    public class ServiceSettingDTO
    {
        public ServiceSettingEnum Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public Guid? ConcurrencyToken { get; set; }
        public object ToSetValuesModel()
        {
            return new
            {
                Name,
                Value,
                Type,
                ConcurrencyToken,
            };
        }
    }
}
