using System;

namespace hb29.API.DTOs
{
    public class UserPreferencesDTO
    {
        public long Id { get; set; }
        public string Upn { get; set; }
        public long PreferenceId { get; set; }
        public bool Value { get; set; }
        public Guid? ConcurrencyToken { get; set; }
        public object ToSetValuesModel()
        {
            return new
            {
                Id,
                Upn,
                PreferenceId,
                Value,
                ConcurrencyToken
            };
        }
    }
}
