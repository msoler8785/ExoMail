using ExoMail.Smtp.Enums;
using ExoMail.Smtp.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
    public class SmtpStartTlsCommand : SmtpCommandBase
    {
        public SmtpStartTlsCommand(string command, List<string> arguments)
        {
            Command = command;
            Arguments = arguments;
        }

        public override bool ArgumentsValid
        {
            get
            {
                return this.Arguments.Count == 0;
            }
        }

        private string GetResponse()
        {
            string response;

            if (this.ArgumentsValid)
            {
                switch (this.SmtpSession.SessionState)
                {
                    case SessionState.EhloNeeded:
                        response = SmtpResponse.SendHelloFirst;
                        break;

                    case SessionState.MailNeeded:
                    case SessionState.StartTlsNeeded:
                        this.IsValid = true;
                        response = SmtpResponse.StartTls;
                        break;

                    default:
                        response = SmtpResponse.BadCommand;
                        break;
                }
            }
            else
            {
                response = SmtpResponse.ArgumentUnrecognized;
            }
            return response;
        }

        public override async Task<string> GetResponseAsync()
        {
            return await Task.Run(() => GetResponse());
        }

        public override async Task ProcessCommandAction()
        {
            if (this.IsValid)
            {
                await this.SmtpSession.StartTlsAsync();
                this.SmtpSession.SessionState = SessionState.EhloNeeded;
                this.SmtpSession.SmtpCommands.Add(this);
            }
        }
    }
}