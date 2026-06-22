using ExoMail.Smtp.Client.Interfaces;
using ExoMail.Smtp.Client.Network;
using System;
using System.Collections.Generic;

namespace ExoMail.Smtp.Client.Authentication
{
    internal class SaslInitializer
    {
        private string UserName { get; set; }
        private string Password { get; set; }
        private SmtpClient SmtpClient { get; set; }

        public SaslInitializer(string userName, string password, SmtpClient smtpClient)
        {
            UserName = userName;
            Password = password;
            SmtpClient = smtpClient;
        }

        public List<ISaslMechanism> GetMechanisms(string ehloResponse)
        {
            List<ISaslMechanism> mechanisms = new List<ISaslMechanism>();

            if (String.IsNullOrEmpty(UserName) && String.IsNullOrEmpty(Password))
                return mechanisms;

            if (ehloResponse.ToUpper().Contains("LOGIN"))
                mechanisms.Add(new LoginSaslMechanism(UserName, Password, SmtpClient));

            if (ehloResponse.ToUpper().Contains("PLAIN"))
                mechanisms.Add(new PlainSaslMechanism(UserName, Password, SmtpClient));

            return mechanisms;
        }
    }
}