using ExoMail.Smtp.Common.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ExoMail.Smtp.Service
{
    public class VirtualServer : IVirtualServer
    {
        public string VirtualServerId { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public List<IDkimPolicy> DkimPolicies { get; set; }

        public string Name { get; set; }
        //private readonly ILoggerFactory _loggerFactory;

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public HashSet<IUserIdentity> Users { get; set; }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public HashSet<IMessageRoute> Routes { get; set; }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public Dictionary<string, string> Options { get; set; }

        public VirtualServer()
        {
            //_loggerFactory = loggerFactory;
            Users = new HashSet<IUserIdentity>();
            Routes = new HashSet<IMessageRoute>();
            DkimPolicies = new List<IDkimPolicy>();
            Options = new Dictionary<string, string>();
        }


        public override string ToString()
        {
            return $"VirtualServerId: {VirtualServerId} Name: {Name} Description: {Description}";
        }

        public override int GetHashCode()
        {
            return VirtualServerId.GetHashCode();
        }
    }
}