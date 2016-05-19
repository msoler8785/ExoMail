using ExoMail.Smtp.Authentication;
using ExoMail.Smtp.Exceptions;
using ExoMail.Smtp.Interfaces;
using ExoMail.Smtp.Utilities;
using System;
using System.Text;

namespace ExoMail.Smtp.Server.Authentication
{
    public class LoginSaslMechanism : SaslMechanismBase, ISaslMechanism
    {

        public bool IsAuthenticated
        {
            get
            {
                return UserManager.GetUserManager.IsUserAuthenticated(this.UserName, this.Password);
            }
        }

        public LoginSaslMechanism()
            :base()
        {
            base.SaslMechanism = "LOGIN";
            base.Step = 0;
            SetInitiator(true);
        }

        public static ISaslMechanism Create()
        {
            return new LoginSaslMechanism();
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
                    SetCompleted(true);
                    SetPasswordResponse(response);
                    break;

                default:
                    SetCompleted(true);
                    break;
            }
        }

        public void Reset()
        {
            this.UserName = null;
            this.Password = null;
            this.Step = 0;
            this.SetCompleted(false);
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