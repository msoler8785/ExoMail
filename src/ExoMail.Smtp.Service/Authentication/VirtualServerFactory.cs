using ExoMail.Data.Models;
using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.QueueProcessor;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExoMail.Smtp.Service.Authentication
{
    public class VirtualServerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<VirtualServerFactory> _logger;
        private readonly MessageRouteFactory _messageRouteFactory;

        public VirtualServerFactory(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, MessageRouteFactory messageRouteFactory)
        {
            _serviceProvider = serviceProvider;
            _logger = loggerFactory.CreateLogger<VirtualServerFactory>();
            _messageRouteFactory = messageRouteFactory;
        }

        public VirtualServer CreateFromConfig(VirtualServerConfig virtualServerConfig)
        {
            var dkimPolicies = virtualServerConfig
                .DkimPolicies
                .Select(dkim =>
                    new DkimPolicy(dkim.Domain,
                    dkim.Selector,
                    dkim.Headers.Split(';', StringSplitOptions.RemoveEmptyEntries),
                    dkim.PrivateKey,
                    dkim.UseDkim))
                    .ToList<IDkimPolicy>();

            var virtualServer = new VirtualServer()
            {
                VirtualServerId = virtualServerConfig.VirtualServerConfigId.ToString(),
                Active = virtualServerConfig.IsActive,
                Name = virtualServerConfig.Name,
                Description = virtualServerConfig.Description,
                Users = virtualServerConfig.Users.Select(user => EmailUser.CreateFromConfig(user)).ToHashSet<IUserIdentity>(),
                Routes = virtualServerConfig.Routes.Select(route => _messageRouteFactory.CreateFromConfig(route)).ToHashSet<IMessageRoute>(),
                DkimPolicies = dkimPolicies,
            };

            _logger.LogInformation("Loading server {0} from database.", virtualServer.ToString());
            return virtualServer;
        }
    }
}
