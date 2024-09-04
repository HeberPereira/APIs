using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hb29.API.DTOs
{
    public class ProfileDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IdmRole { get; set; }
        public string AdGroupId { get; set; }
        public virtual ICollection<PermissionDTO> Permissions { get; set; } = new List<PermissionDTO>();
        public Guid? ConcurrencyToken { get; set; }
        public object ToSetValuesModel()
        {
            return new
            {
                Name,
                Description,
                IdmRole,
                AdGroupId,
                ConcurrencyToken
            };
        }
    }
}
