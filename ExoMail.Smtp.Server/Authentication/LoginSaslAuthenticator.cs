using ExoMail.Smtp.Authentication;
using ExoMail.Smtp.Exceptions;
using ExoMail.Smtp.Interfaces;
using ExoMail.Smtp.Utilities;
using System;
using System.Text;

namespace ExoMail.Smtp.Server.Authentication
{
    public class LoginSaslAuthenticator : ISaslAuthenticator
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public bool IsAuthenticated
        {
            get
            {
                return UserManager.GetUserManager.IsUserAuthenticated(this.UserName, this.Password);
            }
        }

        public string SaslMechanism { get; set; }
        public bool IsCompleted { get; private set; }
        public bool IsInitiator { get; private set; }
        public int Step { get; set; }

        //private List<IUserStore> UserStores { get; set; }
        public LoginSaslAuthenticator()
        {
            this.SaslMechanism = "LOGIN";
            this.IsCompleted = false;
            this.IsInitiator = true;
            this.Step = 0;
            //this.UserStore = userStore;
        }

        public ISaslAuthenticator Create()
        {
            return new LoginSaslAuthenticator();
        }

        public string GetChallenge()
        {
            switch (this.Step)
            {
                case 0:
                    return SmtpResponse.AuthLoginUserName;

                case 1:
                    return SmtpResponse.AuthLoginPassword;

                default:
                    return SmtpResponse.AuthCredInvalid;
            }
        }

        public void ParseResponse(string response)
        {
            switch (this.Step)
            {
                case 0:
                    this.Step++;
                    SetUserNameResponse(response);
                    break;

                case 1:
                    this.Step++;
                    this.IsCompleted = true;
                    SetPasswordResponse(response);
                    break;

                default:
                    this.IsCompleted = true;
                    break;
            }
        }

        public void Reset()
        {
            this.UserName = null;
            this.Password = null;
            this.Step = 0;
            this.IsCompleted = false;
        }

        private void SetUserNameResponse(string userNameResponse)
        {
            if (userNameResponse == null)
            {
                throw new SaslException("UserName cannot be null.");
            }
            this.UserName = Encoding.UTF8.GetString(Convert.FromBase64String(userNameResponse));
        }

        private void SetPasswordResponse(string passwordResponse)
        {
            if (String.IsNullOrEmpty(passwordResponse))
            {
                throw new SaslException("Password cannot be null or empty.");
            }
            this.Password = Encoding.UTF8.GetString(Convert.FromBase64String(passwordResponse));
        }
    }
}