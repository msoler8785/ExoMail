using ExoMail.Smtp.Common.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExoMail.Smtp.Common.Utilities
{
    public class SystemVirtualServer : IVirtualServer
    {
        public SystemVirtualServer()
        {
            VirtualServerId = Guid.Empty.ToString();
            Name = "System Server";
            Description = "The default system server.";
            DkimPolicies = new List<IDkimPolicy>();
            Users = new HashSet<IUserIdentity>()
            {
                new SystemIdentity()
            };

            Routes = new HashSet<IMessageRoute>()
            {
                new SystemRoute()
            };

            Options = new Dictionary<string, string>();
        }

        public string VirtualServerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; } = true;

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public List<IDkimPolicy> DkimPolicies { get; set; }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public HashSet<IUserIdentity> Users { get; set; }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public HashSet<IMessageRoute> Routes { get; set; }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public Dictionary<string, string> Options { get; set; }

        public long Sequence { get; set; }
    }
}
