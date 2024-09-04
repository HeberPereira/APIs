using hb29.API.Models;
using hb29.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace hb29.API.Repository
{
    //Audit Log implementation
    public partial class DefaultContext
    {
        private AzureQueueStorage _queueService { get; set; }
        private const string queueNameAuditLog = "audit-logs";

        public void SetQueueService(AzureQueueStorage queueService)
        {
            _queueService = queueService;
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            DateTime changeTime = DateTime.UtcNow;

            //get changed, added and deleted 
            var changedEntries = this.ChangeTracker.Entries().Where(p =>
                p.State == EntityState.Added ||
                p.State == EntityState.Deleted ||
                p.State == EntityState.Modified)
                .Where(e => e.Entity.GetType().IsDefined(typeof(Models.Attributes.AuditLogAttribute), false))
                .Select(e => new AuditLogEntity
                {
                    EntityEntry = e,
                    PreviousState = e.State
                })
                .ToList();

            try
            {

                var newEntriesValidated = this.ChangeTracker.Entries().Where(p => p.State == EntityState.Added);
                if (newEntriesValidated.Any())
                {
                    foreach (EntityEntry entry in newEntriesValidated)
                    {
                        if (entry.Metadata.FindProperty("CreatedAt") != null)
                            entry.Property("CreatedAt").CurrentValue = DateTime.Now;

                        if (entry.Metadata.FindProperty("ConcurrencyToken") != null)
                            entry.Property("ConcurrencyToken").CurrentValue = Guid.NewGuid();

                        if (entry.Metadata.FindProperty("CreatedBy") != null)
                        {
                            try
                            {
                                entry.Property("CreatedBy").CurrentValue = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.Upn);
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                var changedEntriesValidated = this.ChangeTracker.Entries().Where(p => p.State == EntityState.Deleted || p.State == EntityState.Modified);
                if (changedEntriesValidated.Any())
                {
                    foreach (EntityEntry entry in changedEntriesValidated)
                    {
                        if (entry.Metadata.FindProperty("UpdatedAt") != null)
                            entry.Property("UpdatedAt").CurrentValue = DateTime.Now;

                        if (entry.Metadata.FindProperty("UpdatedBy") != null)
                        {
                            try
                            {
                                entry.Property("UpdatedBy").CurrentValue = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.Upn);
                            }
                            catch
                            {
                            }
                        }
                        if (entry.Metadata.FindProperty("ConcurrencyToken") != null)
                        {
                            entry.Property("ConcurrencyToken").OriginalValue = entry.CurrentValues["ConcurrencyToken"];
                            entry.Property("ConcurrencyToken").CurrentValue = Guid.NewGuid();
                        }
                    }
                }
                // Call the original SaveChanges(), which will save changes made.
                // This must be done here, as new records needs to receive a valid ID.
                //TODO: vale criar transaction para o caso do auditlog gerar erro?
                var saveChangesResult = await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();
                var databaseEntry = exceptionEntry.GetDatabaseValues();
                if (databaseEntry == null || ((ModelBase)databaseEntry.ToObject()).DeletedAt != null)
                {
                    throw new Exception("Unable to save. The entity was deleted by another user.");
                }

                throw new Exception("The record you attempted to edit was modified by another user after you. The edit operation was canceled.");
            }


            // Get all Added/Deleted/Modified entities (not Unmodified or Detached)
            List<AuditLog> listaQueue = new();
            foreach (var ent in changedEntries)
            {
                //check if entity is marked to be auditable
                if (!ent.EntityEntry.Entity.GetType().IsDefined(typeof(Models.Attributes.AuditLogAttribute), false))
                    continue;

                // For each changed record, get the audit record entries and add them
                var queueItem = GetAuditLogItem(ent.EntityEntry, changeTime, ent.PreviousState);
                await _queueService.InsertMessage(System.Text.Json.JsonSerializer.Serialize(queueItem), queueNameAuditLog);
                listaQueue.Add(queueItem);
            }

            //save audits
            return await base.SaveChangesAsync(cancellationToken);
        }


        private AuditLog GetAuditLogItem(EntityEntry dbEntry, DateTime changeTime, EntityState state)
        {
            // Get table name (if it has a Table attribute, use that, otherwise get the pluralized name)
            string tableName = dbEntry.Entity.GetType().Name;

            // Get primary key value (If you have more than one key column, this will need to be adjusted)
            var properties = dbEntry.Entity.GetType()
                .GetProperties();

            var keyName = properties
                .Where(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0)
                .Select(p => p.Name)
                .FirstOrDefault();

            bool existsDeletedAt = properties
                .Where(p => p.Name == "DeletedAt")
                .Any();

            //set event type
            string eventType = state switch
            {
                EntityState.Added => "I",
                EntityState.Deleted => "D",
                EntityState.Modified => existsDeletedAt && dbEntry.CurrentValues.GetValue<DateTime?>("DeletedAt").HasValue ? "D" : "M",
                _ => throw new ArgumentException("Invalid entity state for AuditLog."),
            };
            long dbEntryId = dbEntry.CurrentValues.GetValue<long>(keyName);

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(
                  dbEntry.Entity,
                  Newtonsoft.Json.Formatting.None,
                  new Newtonsoft.Json.JsonSerializerSettings
                  {
                      ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                      MaxDepth = 2
                  }
            );

            var auditLogItem = new AuditLog()
            {
                //Id = Guid.NewGuid(),
                EventDate = changeTime,
                EventType = eventType,
                TableName = tableName,
                RecordId = dbEntryId < 0 ? 0 : dbEntryId,
                NewValue = json //JsonSerializer.Serialize(dbEntry.Entity, jsonSerializerOptions)
            };

            return auditLogItem;
        }
    }
}
