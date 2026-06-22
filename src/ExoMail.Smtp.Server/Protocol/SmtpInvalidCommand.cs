using ExoMail.Smtp.Server.Enums;
using ExoMail.Smtp.Server.Utilities;
using System;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Protocol
{
    public class SmtpInvalidCommand : SmtpCommandBase
    {
        public SmtpInvalidCommand(SmtpSession session)
        {
            this.SmtpSession = session;
        }

        public override SessionState AllowedStates
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override async Task ExecuteAsync()
        {
            string response;
            ValidateArgs(out response);

            await SmtpSession.SendResponseAsync(response);
        }

        public override bool ValidateArgs(out string argumentsResponse)
        {
            argumentsResponse = String.Empty;

            switch (this.SmtpSession.SmtpStateTable.SessionState)
            {
                case SessionState.EhloNeeded:
                    argumentsResponse = SmtpResponse.SendHelloFirst;
                    break;

                case SessionState.StartTlsNeeded:
                    argumentsResponse = SmtpResponse.StartTlsFirst;
                    break;

                case SessionState.MailNeeded:
                    argumentsResponse = SmtpResponse.SenderFirst;
                    break;

                case SessionState.RcptNeeded:
                    argumentsResponse = SmtpResponse.SenderAndRecipientFirst;
                    break;

                case SessionState.DataNeeded:
                    break;

                case SessionState.AuthNeeded:
                    argumentsResponse = SmtpResponse.AuthRequired;
                    break;

                default:
                    argumentsResponse = SmtpResponse.BadCommand;
                    break;
            }

            return true;
        }
    }
}