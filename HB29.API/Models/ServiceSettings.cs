
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace hb29.API.Models
{
    public enum ServiceSettingEnum
    {
        
        Configuracao1 = 1,
        Configuracao2 = 2,
        Configuracao3 = 3
    }

    public class ServiceSetting
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ServiceSettingEnum Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        [Required, MaxLength(255)]
        public string Value { get; set; }

        [Required, MaxLength(15)]
        public string Type { get; set; }

        public Guid? ConcurrencyToken { get; set; }

        public ServiceSetting()
        {
            this.ConcurrencyToken = new Guid();
        }

        public DTOs.ServiceSettingDTO ToDTO()
        {
            return new()
            {
                Id = Id,
                Name = Name,
                Type = Type,
                Value = Value,
                ConcurrencyToken = this.ConcurrencyToken,
            };
        }
    }    
}
