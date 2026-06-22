using ExoMail.Smtp.Common.Interfaces;
using System.Collections.Generic;

namespace ExoMail.Smtp.Common.Interfaces
{
    public interface IVirtualServer
    {
        string VirtualServerId { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        bool Active { get; set; }
        List<IDkimPolicy> DkimPolicies { get; }
        HashSet<IUserIdentity> Users { get; }
        HashSet<IMessageRoute> Routes { get; }
        Dictionary<string, string> Options { get; set; }
    }
}