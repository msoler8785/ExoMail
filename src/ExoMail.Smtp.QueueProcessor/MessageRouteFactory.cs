using ExoMail.Data.Models;
using ExoMail.Smtp.Common.Extensions;
using ExoMail.Smtp.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace ExoMail.Smtp.QueueProcessor
{
    public class MessageRouteFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MessageRouteFactory> _logger;

        public MessageRouteFactory(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            _serviceProvider = serviceProvider;
            _logger = loggerFactory.CreateLogger<MessageRouteFactory>(); 
        }

        public IMessageRoute CreateFromConfig(MessageRouteConfig messageRouteConfig)
        {
            var route = new MessageRoute(messageRouteConfig.Destination);
            route.Metric = messageRouteConfig.Metric;
            route.MessageRouteId = messageRouteConfig.MessageRouteId.ToString();
            route.Destination = messageRouteConfig.Destination;
            
            var gatewayType = Type.GetType(messageRouteConfig.TypeName);
            //route.MessageGateway = _serviceProvider.GetRequiredService(gatewayType);
            route.MessageGateway = _serviceProvider.GetRequiredService<IMessageGateway>(gatewayType);

            var gatewayOptions = new GatewayOptions()
            {
                MaxClientsPerDomain = messageRouteConfig.MaxClientsPerDomain,
                MaxMessageRetries = messageRouteConfig.MaxMessageRetries,
                MaxMessagesPerSession = messageRouteConfig.MaxMessagesPerSession,
                MessageDeferralTime = messageRouteConfig.MessageDeferralTime
            };

            route.MessageGateway.GatewayOptions = gatewayOptions;

            return route;
        }
    }
}
