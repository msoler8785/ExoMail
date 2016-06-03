using ExoMail.Smtp.Interfaces;
using ExoMail.Smtp.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Example
{
    public class EmailUser : IUserIdentity
    {
        public List<string> AliasAddresses { get; set; }

        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        private List<string> _folders;
        public List<string> Folders
        {
            get
            {
                if (Directory.Exists(this.MailboxPath))
                {
                    return Directory.EnumerateDirectories(this.MailboxPath, "*", SearchOption.AllDirectories).ToList();
                }
                else
                {
                    return _folders;
                }
            }
        }

        public bool IsActive { get; set; }

        public string LastName { get; set; }

        public string MailboxPath { get; set; }

        public long MailboxSize { get; set; }

        public long MaxMailboxSize { get; set; }

        public long MaxMessageSize { get; set; }

        public long MessageCount { get; set; }

        public string Password { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }

        public EmailUser()
        {
            this._folders = new List<string>();
            this.AliasAddresses = new List<string>();
        }

        public static EmailUser CreateMailbox(string firstName, string lastName, string emailAddress)
        {
            var emailUser = new EmailUser()
            {
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress,
                IsActive = true,
                MailboxPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users", emailAddress),
                MaxMessageSize = ByteSizeHelper.FromMegaBytes(10),
                UserId = Guid.NewGuid().ToString(),
                UserName = emailAddress,
                Password = "UnhashedPassword"
            };

            return emailUser;
        }
    }
}
