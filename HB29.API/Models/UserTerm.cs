using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hb29.API.Models
{
    public class UserTerm : ModelBase
    {
        [Required]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Privacy Policy is required.")]
        [ForeignKey("PrivacyPolicy")]
        public long PrivacyPolicyId { get; set; }
        public virtual PrivacyPolicy Cluster { get; set; }
    }
}
