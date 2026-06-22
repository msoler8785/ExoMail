using ExoMail.Smtp.Common.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ExoMail.Smtp.Common.Utilities
{
    public class SystemIdentity : IUserIdentity
    {
        public SystemIdentity()
        {
            UserId = Guid.Empty.ToString();
            UserName = "SYSTEM";
            EmailAddresses = new List<string>() { "postmaster" };
            FirstName = "System";
            LastName = "User";
            MailboxPath = Path.Combine("System", "Inbox");
            
            Directory.CreateDirectory(MailboxPath);
            VirtualServerId = Guid.Empty.ToString();
        }

        public string UserId { get; private set; }
        public string UserName { get; private set; }
        public List<string> EmailAddresses { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MailboxPath { get; private set; }

        public long MaxMessageSize => 102400;
        public long MaxMailboxSize => 0;
        public long MailboxSize => 0;
        public long MessageCount => 0;
        public bool IsActive => true;
        
        [JsonIgnore]
        public List<string> Folders { get; private set; }
        public string VirtualServerId { get; private set;}

        public void SetPassword(string password)
        {
            throw new NotImplementedException();
        }

        public bool VerifyPassword(string password)
        {
            throw new NotImplementedException();
        }
    }
}
