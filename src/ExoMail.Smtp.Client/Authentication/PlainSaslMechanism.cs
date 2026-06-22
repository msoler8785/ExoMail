using ExoMail.Smtp.Client.Enums;
using ExoMail.Smtp.Client.Interfaces;
using ExoMail.Smtp.Client.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Client.Authentication
{
    public class PlainSaslMechanism : ISaslMechanism
    {
        public PlainSaslMechanism(string userName, string password, SmtpClient smtpClient)
        {
            UserName = userName;
            Password = password;
            SmtpClient = smtpClient;
        }

        public string UserName { get; private set; }
        public string Password { get; private set; }
        public SmtpClient SmtpClient { get; private set; }

        public bool IsComplete { get; private set; }

        public async Task InitiateAuth()
        {
            string challenge = Convert.ToBase64String(Encoding.ASCII.GetBytes($"\0{UserName}\0{Password}"));
            string commmand = $"AUTH PLAIN {challenge}";
            await SmtpClient.SendCommandAsync(commmand);

            //string response = await SmtpClient.ListenResponseAsync();
            //IExecutionResult result = ExecutionResult.Parse(response);
            IsComplete = true;
        }
    }
}
