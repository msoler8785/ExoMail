using ExoMail.Smtp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Authentication
{

    public class JsonUserIdentity : IUserIdentity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Realm { get; set; }

        //Return true if UserName and Password combination exists in UserStore.
        public bool IsAuthenticated { get; set; }

        //Return true if user is allowed to login.
        public bool IsAuthorized { get; set; }

        public JsonUserIdentity()
        {
            this.IsAuthenticated = false;
            this.IsAuthorized = false;
            this.Realm = String.Empty;
        }
    }
}
