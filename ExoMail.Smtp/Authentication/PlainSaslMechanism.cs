using ExoMail.Smtp.Exceptions;
using ExoMail.Smtp.Interfaces;
using ExoMail.Smtp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Authentication
{
    /// <summary>
    /// The PLAIN Simple Authentication and Security Layer (SASL) Mechanism.
    /// <see cref="https://tools.ietf.org/html/rfc4616" />
    /// </summary>
    public class PlainSaslMechanism : SaslMechanismBase, ISaslMechanism
    {
        public bool IsAuthenticated
        {
            get
            {
                return UserManager.GetUserManager.IsUserAuthenticated(this.UserName, this.Password);
            }
        }

        public PlainSaslMechanism()
            : base()
        {
            base.SaslMechanism = "PLAIN";
            base.Step = 0;
            SetInitiator(true);
        }

        public string GetChallenge()
        {
            return SmtpResponse.AuthStart;
        }

        public void ParseResponse(string response)
        {
            ProcessResponse(response);
            SetCompleted(true);
        }

        private void ProcessResponse(string response)
        {
            // Convert from Base64 string.
            response = Encoding.UTF8.GetString(Convert.FromBase64String(response));

            // Get the credential constituents
            string[] credential = response.Split(new char[] { '\0' }, StringSplitOptions.None);

            switch (credential.Length)
            {
                case 0:
                case 1:
                    throw new SaslException("Invalid credential parameters.");
                case 3:
                    {
                        if (credential[0] != credential[1])
                        {
                            throw new SaslException("Not authorized to requested authorization identity.");
                        }
                    }
                    break;
                default:
                    throw new SaslException();
            }

            string userName = credential[credential.Length - 2];
            string password = credential[credential.Length - 1];

            if (userName == null || String.IsNullOrEmpty(password))
            {
                throw new SaslException("UserName cannot be null and Password cannot be null or empty.");
            }

            this.UserName = userName;
            this.Password = password;
        }
    }
}
