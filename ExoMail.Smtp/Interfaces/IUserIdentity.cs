using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Interfaces
{
    public interface IUserIdentity
    {
        string UserName { get; set; }
        string Password { get; set; }

        //Return true if UserName and Password combination exists in UserStore.
        bool IsAuthenticated { get; set; }

        //Return true if user is allowed to login.
        bool IsAuthorized { get; set; }
    }
}
