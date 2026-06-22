using DnsClient;
using ExoMail.Smtp.Common;
using ExoMail.Smtp.Common.Extensions;
using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.PluginExample;
using ExoMail.Smtp.QueueProcessor;
using ExoMail.Smtp.RspamdClient;
using ExoMail.Smtp.Sdk;
using ExoMail.Smtp.Server.Interfaces;
using ExoMail.Smtp.Server.Network;
using ExoMail.Smtp.Service.Configuration;
using ExoMail.Data;
using ExoMail.Smtp.Service.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ExoMail.Smtp.Service.Authentication;

namespace ExoMail.Smtp.Service
{
    public class Service
    {
        public IConfigurationRoot Configuration { get; set; }
        private ISmtpServerRepository _smtpServerRepository;
        public bool IsRunning { get; private set; }

        public Service()
        {
            //this._token = token;
        }

        public async Task Start()
        {
            IServiceProvider serviceProvider = ConfigureServices();
            serviceProvider.GetRequiredService<ILoggerFactory>().AddNLog();

            await serviceProvider.GetRequiredService<IStartup>().Initialize();
            _smtpServerRepository = serviceProvider.GetRequiredService<ISmtpServerRepository>();

            IsRunning = true;
            await _smtpServerRepository.Start();
            
            IsRunning = false;
        }

        public async Task Stop()
        {
            await _smtpServerRepository.Stop();
        }

        private IServiceProvider ConfigureServices()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
            IServiceCollection serviceCollection = new ServiceCollection()

            // Register serialization dependencies
            .AddTransient<MessageRoute>()
            .AddTransient<MxMessageGateway>()
            .AddTransient<NullMessageGateway>()

            // Register the service dependencies
            .AddSingleton<IStartup, Startup>()
            .AddSingleton<ILookupClient, LookupClient>()
            .AddSingleton<IVirtualServerRepository, JsonVirtualServerRepository>()
            .AddSingleton<ISmtpServerRepository, SmtpServerRepository>()
            .AddSingleton<ISmtpServerBuilder, SmtpServerBuilder>()
            .AddSingleton<IMessageStore, FileMessageStore>()
            .AddSingleton<IMessageEnvelopeRepository, MessageEnvelopeRepository>()
            .AddSingleton<ISmtpClientResolver, SmtpClientResolver>()
            .AddSingleton<IDefaultRouter, DefaultRouter>()
            .AddSingleton<IRouteTable, RouteTable>()
            .AddSingleton<IRouteRepository, RouteRepository>()
            .AddSingleton<IMessageProcessor, ClickTrackingProcessor>()
            .AddSingleton<IMessageProcessor, MessageFooterProcessor>()
            .AddSingleton<IMessageProcessor, MessageScoreProcessor>()
            .AddSingleton<DependencyContractResolver>()
            .AddSingleton<MessageRouteFactory>()
            .AddSingleton<VirtualServerFactory>()
            .AddOptions()
            .Configure<QueueOptions>(Configuration.GetSection("QueueOptions"))
            .Configure<RepositoryOptions>(Configuration.GetSection("RepositoryOptions"))
            .Configure<PluginOptions>(Configuration.GetSection("PluginOptions"))
            .Configure<SpamScanningOptions>(Configuration.GetSection("SpamScanningOptions"))
            .AddLogging(x =>
                x.AddConfiguration(Configuration.GetSection("Logging"))
                .AddFilter("ExoMail", LogLevel.Trace)
                .AddConsole())
            .AddResolver()
            .AddDbContext<ConfigContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ConfigConnection")))

            // Factory service dependencies
            .AddTransient<ISmtpServer, SmtpServer>();

            // Import Plugin services
            //.ImportPlugins<IMesssageProcessor>();

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}