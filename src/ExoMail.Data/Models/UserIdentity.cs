using System;
using System.Collections.Generic;

namespace ExoMail.Data.Models
{
    public class UserIdentity
    {
        public Guid UserIdentityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public string MailboxPath { get; set; }
        public long MailboxSize { get; set; }
        public long MaxMailboxSize { get; set; }
        public long MaxMessageSize { get; set; }
        public long MessageCount { get; set; }
        public string UserName { get; set; }

        public Guid VirtualServerConfigId { get; set; }
        public virtual VirtualServerConfig VirtualServerConfigs { get; set; }

        public virtual List<EmailAddress> EmailAddresses { get; set; }
        public virtual List<Folder> Folders { get; set; }
        public string Password { get; set; }

        public UserIdentity()
        {
            EmailAddresses = new List<EmailAddress>();
            Folders = new List<Folder>();
        }
    }
}