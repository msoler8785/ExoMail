using ExoMail.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ExoMail.Dev
{
    public class Service
    {
        public IConfigurationRoot Configuration { get; set; }
        public bool IsRunning { get; private set; }

        public Service()
        {
        }

        public async Task Start()
        {
            IServiceProvider serviceProvider = ConfigureServices();
            serviceProvider.GetRequiredService<ILoggerFactory>()
                .AddNLog();

            var dbInit = serviceProvider.GetRequiredService<DbInitializer>();

            dbInit.SeedDatabase();
        }

        public async Task Stop()
        {
        }

        private IServiceProvider ConfigureServices()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
            IServiceCollection serviceCollection = new ServiceCollection()

            .AddSingleton<DbInitializer>()
            // Register the service dependencies
            .AddOptions()
            .AddLogging(x =>
                x.AddConfiguration(Configuration.GetSection("Logging"))
                .AddFilter("ExoMail", LogLevel.Trace)
                .AddConsole())
            .AddDbContext<ConfigContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ConfigConnection")));

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}