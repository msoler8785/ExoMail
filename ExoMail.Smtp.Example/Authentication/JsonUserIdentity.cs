using ExoMail.Smtp.Interfaces;
using System;

namespace ExoMail.Smtp.Server.Authentication
{
    public class JsonUserIdentity : IUserIdentity
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Realm { get; set; }
        public string EmailAddress { get; set; }

        public JsonUserIdentity()
        {
            this.Realm = String.Empty;
            this.UserId = Guid.NewGuid().ToString();
        }
    }
}