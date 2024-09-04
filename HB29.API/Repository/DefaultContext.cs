using hb29.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;


namespace hb29.API.Repository
{
    public partial class DefaultContext : DbContext
    {
        private IHttpContextAccessor _httpContextAccessor;
        public DefaultContext(DbContextOptions<DefaultContext> options, IHttpContextAccessor httpContextAccessor = null)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            SeedPermissionData(modelbuilder);
            SeedServiceSettingData(modelbuilder);

            base.OnModelCreating(modelbuilder);
        }

        private void SeedPermissionData(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<Permission>().HasData(
                new Permission() { Id = 103, Name = "START_ACTIVITY", Description = "START_ACTIVITY" },
                new Permission() { Id = 104, Name = "DOWNLOAD_FILE_ACTIVITY", Description = "DOWNLOAD_FILE_ACTIVITY" },
                new Permission() { Id = 105, Name = "UPDATE_CLUSTER", Description = "UPDATE_CLUSTER" },
                new Permission() { Id = 106, Name = "SAVE_CLUSTER", Description = "SAVE_CLUSTER" },
                new Permission() { Id = 107, Name = "REMOVE_CLUSTER", Description = "REMOVE_CLUSTER" },
                new Permission() { Id = 108, Name = "UPDATE_COUNTRY", Description = "UPDATE_COUNTRY" },
                new Permission() { Id = 109, Name = "SAVE_COUNTRY", Description = "SAVE_COUNTRY" }
              
                );
        }

        private void SeedServiceSettingData(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<ServiceSetting>().HasData(
                new ServiceSetting()
                {
                    Id = ServiceSettingEnum.Configuracao1,
                    Name = "Configuração 1",
                    Type = typeof(int).Name,
                    Value = "30"
                },
                //MAIL SETTINGS GRAPH
                new ServiceSetting()
                {
                    Id = ServiceSettingEnum.Configuracao2,
                    Name = "Configuração 2",
                    Type = typeof(string).Name,
                    Value = ""
                },
                new ServiceSetting()
                {
                    Id = ServiceSettingEnum.Configuracao3,
                    Name = "Configuração 3",
                    Type = typeof(string).Name,
                    Value = ""
                }
            );
        }

        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<ServiceSetting> ServiceSettings { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PrivacyPolicy> PrivacyPolicies { get; set; }
        public DbSet<UserTerm> UserTerms { get; set; }
    }
}