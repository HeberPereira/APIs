
using hb29.API.Enums;
using System;
using System.Collections.Generic;

namespace hb29.API.DTOs
{
    public class PrivacyPolicyDTO
    {
        public long Id { get; set; }
        public PrivacyPolicyStatusEnum PrivacyPolicyStatus { get; set; }
        public string conditions { get; set; }
        public Guid? ConcurrencyToken { get; set; }
        public object ToSetValuesModel()
        {
            return new
            {
                PrivacyPolicyStatus,
                conditions,
                ConcurrencyToken
            };
        }
    }
}
