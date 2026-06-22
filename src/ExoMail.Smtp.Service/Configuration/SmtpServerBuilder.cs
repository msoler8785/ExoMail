using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Server.Interfaces;
using ExoMail.Smtp.Server.Network;
using ExoMail.Smtp.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Service.Configuration
{
    public class SmtpServerBuilder : ISmtpServerBuilder
    {
        private readonly ILogger<SmtpServerBuilder> _logger;
        private readonly IEnumerable<ISmtpServerConfig> _configurations;
        private readonly IServiceProvider _serviceProvider;

        public SmtpServerBuilder(ILoggerFactory loggerFactory, ISmtpConfigProvider provider, IServiceProvider serviceProvider)
        {
            _logger = loggerFactory.CreateLogger<SmtpServerBuilder>();
            _configurations = provider.Load();
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<Task> Build(CancellationToken token)
        {
            _logger.LogTrace("Begin {0}", nameof(Build));

            var servers = _configurations
                .Select(async x => await _serviceProvider.GetRequiredService<ISmtpServer>().StartAsync(x, token));

            return servers;
        }
    }
}