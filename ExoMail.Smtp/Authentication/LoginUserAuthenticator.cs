using ExoMail.Smtp.Interfaces;
using ExoMail.Smtp.Utilities;
using System;
using System.Text;

namespace ExoMail.Smtp.Authentication
{
    public class LoginUserAuthenticator : IUserAuthenticator
    {
        public bool IsAuthenticated
        {
            get
            {
                return this.UserStore.IsUserAuthenticated(this.UserName, this.Password);
            }
        }

        public string Password { get; set; }
        public string SaslMechanism { get; set; }
        public string UserName { get; set; }
        public string UserNameChallenge { get; set; }
        public string PasswordChallenge { get; set; }
        public IUserStore UserStore { get; set; }

        public LoginUserAuthenticator()
        {
            this.SaslMechanism = "LOGIN";
            this.UserNameChallenge = SmtpResponse.AuthLoginUserName;
            this.PasswordChallenge = SmtpResponse.AuthLoginPassword;
        }

        public void SetUserNameResponse(string userNameResponse)
        {
            this.UserName = Encoding.UTF8.GetString(Convert.FromBase64String(userNameResponse));
        }

        public void SetPasswordResponse(string passwordResponse)
        {
            this.Password = Encoding.UTF8.GetString(Convert.FromBase64String(passwordResponse));
        }
    }
}