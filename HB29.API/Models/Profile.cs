using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using hb29.API.DTOs;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace hb29.API.Models
{
    [Index(nameof(Name), IsUnique = true, Name = "IX_Profiles_Name_Unique")]
    [Index(nameof(AdGroupId), IsUnique = true, Name = "IX_Profiles_AddGroupName_Unique")]
    public class Profile : ModelBase
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(150)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(255)]
        public string Description { get; set; }
        
        [Required(ErrorMessage = "Active Diretocry (AD) Group ID is required.")]
        [MaxLength(150)]
        public string AdGroupId { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();


        public ProfileDTO ToDTO()
        {
            return new()
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                AdGroupId = this.AdGroupId,
                ConcurrencyToken = this.ConcurrencyToken,
            };
        }
    }   
}
