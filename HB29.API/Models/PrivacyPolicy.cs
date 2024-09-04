using hb29.API.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace hb29.API.Models
{
    public class PrivacyPolicy : ModelBase
    {
        [Required]
        public PrivacyPolicyStatusEnum PrivacyPolicyStatus { get; set; }

        [Required]
        public string conditions { get; set; }
    }
}
