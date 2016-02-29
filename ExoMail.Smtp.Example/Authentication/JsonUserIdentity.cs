using ExoMail.Smtp.Interfaces;
using System;

namespace ExoMail.Smtp.Server.Authentication
{
    public class JsonUserIdentity : IUserIdentity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Realm { get; set; }

        public JsonUserIdentity()
        {
            this.Realm = String.Empty;
        }
    }
}