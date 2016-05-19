using System.Collections.Generic;

namespace ExoMail.Smtp.Interfaces
{
    public interface IUserStore
    {
        string Domain { get; set; }
        bool IsUserAuthenticated(string userName, string password);
        bool IsValidRecipient(string emailAddress);
        List<IUserIdentity> GetIdentities();
        void AddUser(IUserIdentity userIdentity);
    }
}