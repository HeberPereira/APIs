using System;

namespace hb29.API.DTOs
{
    public class PreferencesDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
        public Guid? ConcurrencyToken { get; set; }
        public object ToSetValuesModel()
        {
            return new
            {   Id,
                Name,
                Description,
                IsEnabled,
                ConcurrencyToken
            };
        }
    }
}
