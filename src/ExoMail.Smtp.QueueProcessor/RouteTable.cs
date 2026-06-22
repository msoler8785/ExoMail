using ExoMail.Smtp.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExoMail.Smtp.QueueProcessor
{
    public class RouteTable : IRouteTable
    {
        private readonly ILogger<RouteTable> _logger;

        public RouteTable(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RouteTable>();
        }

        public IMessageRoute FindRoute(string domain, IVirtualServer virtualServer)
        {
            IMessageRoute route = virtualServer
                .Routes
                .OrderBy(x => x.Metric)
                .FirstOrDefault(x => x.IsMatch(domain));

            return route;
        }
    }
}
