using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Interfaces
{
    public interface IUserStore
    {
        List<IUserIdentity> Identities { get; set; }
        string Domain { get; set; }
        bool IsUserAuthenticated(string userName, string password);
        bool IsValidRecipient(string emailAddress);
    }
}
