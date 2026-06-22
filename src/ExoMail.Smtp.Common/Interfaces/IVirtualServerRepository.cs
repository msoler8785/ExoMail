using System.Collections.Generic;

namespace ExoMail.Smtp.Common.Interfaces
{
    public interface IVirtualServerRepository : IRepository<IVirtualServer>
    {
        List<IVirtualServer> Items { get; }

        IUserIdentity FindById(string userId);

        IUserIdentity FindByUsername(string userName);

        IUserIdentity FindByEmailAddress(string emailAddress);

        bool TryAuthenticate(string userName, string password, out IUserIdentity userIdentity);

        IVirtualServer FindUserVirtualServer(string userId);
    }
}