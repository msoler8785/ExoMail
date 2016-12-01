using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Interfaces
{
    public interface IUserIdentity
    {
        string UserId { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string EmailAddress { get; set; }
        List<string> AliasAddresses { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string MailboxPath { get; }
        long MaxMessageSize { get; }
        long MaxMailboxSize { get; }
        long MailboxSize { get; }
        long MessageCount { get; }
        bool IsActive { get; }
        List<string> Folders { get; }
    }
}
