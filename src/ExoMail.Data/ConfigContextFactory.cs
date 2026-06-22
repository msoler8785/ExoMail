using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExoMail.Data
{
    public class ConfigContextFactory : IDesignTimeDbContextFactory<ConfigContext>
    {
        public ConfigContext CreateDbContext(string[] args)
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();
            var connectionString = configuration.GetConnectionString("ConfigConnection");

            var optionsBuilder = new DbContextOptionsBuilder<ConfigContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ConfigContext(optionsBuilder.Options);
        }
    }
}
