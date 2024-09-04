using System;

namespace hb29.API.DTOs
{
    public class PreferencesAllByUserDTO
    {
        public long Id { get; set; }
        public long PreferenceId { get; set; }
        public string Name { get; set; }
        public bool Value { get; set; }
        public bool Active { get; set; }
        public Guid? ConcurrencyToken { get; set; }
        public object ToSetValuesModel()
        {
            return new
            {
                Id,
                PreferenceId,
                Name,
                Value,
                Active,
                ConcurrencyToken
            };
        }
    }
}
