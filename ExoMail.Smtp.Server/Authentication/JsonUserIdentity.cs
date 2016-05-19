using ExoMail.Smtp.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace ExoMail.Smtp.Server.Authentication
{
    public class JsonUserIdentity : IUserIdentity
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }

        public List<string> AliasAddresses { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MailboxPath { get; set; }

        public JsonUserIdentity()
        {
            this.UserId = Guid.NewGuid().ToString();
        }
    }
}