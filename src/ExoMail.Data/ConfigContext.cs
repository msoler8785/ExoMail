using ExoMail.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ExoMail.Data
{
    public class ConfigContext : DbContext
    {
        public DbSet<DkimPolicyConfig> DkimPolicyConfig { get; set; }
        public DbSet<EmailAddress> EmailAddresses { get; set; }
        public DbSet<UserIdentity> UserIdentities { get; set; }
        public DbSet<VirtualServerConfig> VirtualServerConfigs { get; set; }
        public DbSet<Folder> Folders { get; set; }

        public ConfigContext()
        {
            // Intentionally left empty
        }

        public ConfigContext(DbContextOptions<ConfigContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);

        }
    }
}
