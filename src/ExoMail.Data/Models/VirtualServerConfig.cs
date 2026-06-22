using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExoMail.Data.Models
{
    public class VirtualServerConfig
    {
        public Guid VirtualServerConfigId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public virtual List<DkimPolicyConfig> DkimPolicies { get; set; }
        public virtual List<UserIdentity> Users { get; set; }
        public virtual List<MessageRouteConfig> Routes { get; set; }

        public VirtualServerConfig()
        {
            DkimPolicies = new List<DkimPolicyConfig>();
            Users = new List<UserIdentity>();
            Routes = new List<MessageRouteConfig>();
        }
    }
}
