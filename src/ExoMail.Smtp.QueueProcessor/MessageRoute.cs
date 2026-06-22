using DotNet.Globbing;
using ExoMail.Data.Models;
using ExoMail.Smtp.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ExoMail.Smtp.QueueProcessor
{
    public class MessageRoute : IMessageRoute
    {
        private Glob _glob;
        private string _destination;
        private readonly IServiceProvider _serviceProvider;

        public string Destination
        {
            get => _destination;
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    _glob = Glob.Parse(value);

                _destination = value;
            }
        }

        public int Metric { get; set; }
        public string VirtualServerId { get; set; }
        public string MessageRouteId { get; set; }
        public IMessageGateway MessageGateway { get; set; }

        public MessageRoute(
            string destination = "*"
            )
        {
            Destination = destination;
        }

        public async Task EnqueueMesssage(IQueuedMessage queuedMessage, string domain)
        {
            await MessageGateway.EnqueueMessage(queuedMessage, domain);
        }

        public bool IsMatch(string domain) => _glob.IsMatch(domain);

        public override int GetHashCode()
        {
            return MessageRouteId?.GetHashCode() ?? base.GetHashCode();
        }

    }
}