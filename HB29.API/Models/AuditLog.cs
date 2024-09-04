using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace hb29.API.Models
{
    [Index(nameof(EventDate), nameof(TableName), nameof(RecordId))]
    public class AuditLog
    {
        public AuditLog()
        {
        }

        [Key]
        public long Id { get; set; }
        [MaxLength(100)]
        public string Username { get; set; }
        [Required]
        public DateTime EventDate { get; set; }
        [Required, MaxLength(1)]
        public string EventType { get; set; }
        [Required, MaxLength(50)]
        public string TableName { get; set; }
        public long RecordId { get; set; }
        [Required]
        public string NewValue { get; set; }
    }

    public class AuditLogEntity
    {
        public Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry EntityEntry { get; set; }
        public EntityState PreviousState { get; set; }
    }
}