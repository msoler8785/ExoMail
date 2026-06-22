using ExoMail.Smtp.Server.Enums;
using ExoMail.Smtp.Server.Utilities;
using System;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Protocol
{
    public sealed class SmtpStartTlsCommand : SmtpCommandBase
    {
        public override SessionState AllowedStates
        {
            get
            {
                return SessionState.MailNeeded | SessionState.StartTlsNeeded;
            }
        }

        public SmtpStartTlsCommand()
            : base()
        {
        }

        public override bool ValidateArgs(out string argumentsResponse)
        {
            argumentsResponse = String.Empty;
            bool result = this.Arguments.Count == 0;

            if (!result)
                argumentsResponse = SmtpResponse.ArgumentUnrecognized;

            return result;
        }

        public override async Task ExecuteAsync()
        {
            string response;

            if (ValidateArgs(out response))
            {
                response = SmtpResponse.StartTls;

                await SmtpSession.SendResponseAsync(response);

                await this.SmtpSession.StartTlsAsync();
                this.SmtpSession.SmtpStateTable.SessionState = SessionState.EhloNeeded;
                this.SmtpSession.SmtpCommands.Add(this);
            }
        }
    }
}