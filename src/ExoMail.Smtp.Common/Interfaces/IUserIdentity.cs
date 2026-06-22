using System.Collections.Generic;

namespace ExoMail.Smtp.Common.Interfaces
{
    public interface IUserIdentity
    {
        string UserId { get; }
        string UserName { get; }
        List<string> EmailAddresses { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string MailboxPath { get; }
        long MaxMessageSize { get; }
        long MaxMailboxSize { get; }
        long MailboxSize { get; }
        long MessageCount { get; }
        bool IsActive { get; }
        List<string> Folders { get; }
        string VirtualServerId { get; }
        void SetPassword(string password);
        bool VerifyPassword(string password);
        //IVirtualServer VirtualServer { get; }
    }
}