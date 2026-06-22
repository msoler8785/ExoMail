using ExoMail.Smtp.Client.Enums;
using ExoMail.Smtp.Client.Interfaces;
using ExoMail.Smtp.Client.Network;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Client.Authentication
{
    internal class LoginSaslMechanism : ISaslMechanism
    {
        //private const string INPUT_USERNAME = "VXNlciBOYW1l";
        //private const string INPUT_PASSWORD = "UGFzc3dvcmQ=";
        public string Name => "LOGIN";

        private string _userName;

        public string UserName
        {
            get
            {
                if (_userName == null)
                    return String.Empty;

                byte[] userName = Encoding.ASCII.GetBytes(_userName);
                return Convert.ToBase64String(userName);
            }
            set
            {
                _userName = value;
            }
        }

        private string _password;

        public string Password
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_password))
                    throw new ArgumentNullException("Password MUST not be null or empty");

                byte[] password = Encoding.ASCII.GetBytes(_password);
                return Convert.ToBase64String(password);
            }
            set
            {
                _password = value;
            }
        }

        public bool IsComplete { get; set; } = false;
        public bool CanInitiate => true;
        private SmtpClient SmtpClient { get; set; }

        public LoginSaslMechanism(string userName, string password, SmtpClient smtpClient)
        {
            UserName = userName;
            Password = password;
            SmtpClient = smtpClient;
        }

        public async Task InitiateAuth()
        {
            string commmand = $"AUTH LOGIN {UserName}";
            await SmtpClient.SendCommandAsync(commmand);

            string response = await SmtpClient.ListenResponseAsync();
            IExecutionResult result = ExecutionResult.Parse(response);

            if (result.ResponseResult != ResponseResult.Input)
            {
                result = await SendUserName();
            }

            if (result.ResponseResult == ResponseResult.Input)
                await SendPassword();

            IsComplete = true;
        }

        private async Task<IExecutionResult> SendUserName()
        {
            await SmtpClient.SendCommandAsync(UserName);
            string response = await SmtpClient.ListenResponseAsync();
            IExecutionResult result = ExecutionResult.Parse(response);

            return result;
        }

        private async Task SendPassword()
        {
            await SmtpClient.SendCommandAsync(Password);
        }
    }
}