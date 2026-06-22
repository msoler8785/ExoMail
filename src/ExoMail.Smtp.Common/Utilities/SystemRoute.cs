using ExoMail.Smtp.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Common.Utilities
{
    public class SystemRoute : IMessageRoute
    {
        public SystemRoute()
        {
            MessageRouteId = Guid.Empty.ToString();
            VirtualServerId = Guid.Empty.ToString();
        }

        public string MessageRouteId { get; set; }
        public int Metric => 1000;
        public string VirtualServerId { get; set; }

        public async Task EnqueueMesssage(IQueuedMessage queuedMessage, string key)
        {
            await Task.Run(() => queuedMessage.MessageData.DeleteMessageData());
        }

        public bool IsMatch(string domain) => true;
    }
}