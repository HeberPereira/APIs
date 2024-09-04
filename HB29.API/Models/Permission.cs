using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace hb29.API.Models
{
    [Index(nameof(Name), IsUnique = true, Name = "IX_Permissions_Name_Unique")]
    public class Permission : ModelBase
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }

        [Required, MaxLength(255)]
        public string Description { get; set; }

        /// <summary>
        /// Lists all profiles that uses this permission.
        /// </summary>
        /// <remarks>One permission can be associated with many profiles.</remarks>
        public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();
    }
}
