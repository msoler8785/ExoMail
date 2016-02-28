using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Interfaces
{
    public interface IUserAuthenticator
    {
        string UserName { get; set; }
        string Password { get; set; }
        bool IsAuthenticated { get; }
        string SaslMechanism { get; set; }
        string UserNameChallenge { get; set; }
        string PasswordChallenge { get; set; }

        void SetUserNameResponse(string userNameResponse);
        void SetPasswordResponse(string passwordResponse);
    }
}
