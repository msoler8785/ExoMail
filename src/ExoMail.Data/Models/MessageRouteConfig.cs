using System.Threading.Tasks;
using ExoMail.Smtp.Common.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExoMail.Data.Models
{
    public class MessageRouteConfig
    {
        [Key]
        public Guid MessageRouteId { get; set; }
        public int Metric { get; set; }
        public string Destination { get; set; }
        public string TypeName { get; set; }
        public int MaxClientsPerDomain { get; set; }
        public int MaxMessagesPerSession { get; set; }
        public int MessageDeferralTime { get; set; }
        public int MaxMessageRetries { get; set; }
        public string RouteType { get; set; }

        public Guid VirtualServerConfigId { get; set; }
        public virtual VirtualServerConfig VirtualServer { get; set; }
    }
}