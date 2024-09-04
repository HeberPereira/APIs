using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hb29.API.DTOs
{
    public class PermissionDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? ConcurrencyToken { get; set; }
        public DTOs.PermissionDTO ToDTO()
        {
            return new()
            {
                Id = Id,
                Name = Name,
                Description = Description,
                ConcurrencyToken = ConcurrencyToken,
            };
        }
    }
}
