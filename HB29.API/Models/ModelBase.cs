using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hb29.API.Models
{
    public abstract class ModelBase
    {
        [Key]
        public long Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        [MaxLength(100)]
        public string CreatedBy { get; set; }
        [MaxLength(100)]
        public string UpdatedBy { get; set; }
        [MaxLength(100)]
        public string DeletedBy { get; set; }
        [ConcurrencyCheck]
        public Guid? ConcurrencyToken { get; set; } 

        public ModelBase()
        {
            this.ConcurrencyToken = new Guid();
        }
    }
}
