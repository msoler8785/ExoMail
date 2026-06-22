using ExoMail.Smtp.Server.Enums;
using ExoMail.Smtp.Server.Exceptions;
using ExoMail.Smtp.Server.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Protocol
{
    public sealed class SmtpAuthCommand : SmtpCommandBase
    {
        public override SessionState AllowedStates =>
            SessionState.AuthNeeded | SessionState.MailNeeded;

        public SmtpAuthCommand()
            : base()
        {
        }

        public override bool ValidateArgs(out string argumentsResponse)
        {
            argumentsResponse = String.Empty;
            bool result = this.Arguments.Count > 0 && this.Arguments.Count <= 2;

            if (!result)
                argumentsResponse = SmtpResponse.ArgumentUnrecognized;

            return result;
        }

        public override async Task ExecuteAsync()
        {
            string response;

            if (ValidateArgs(out response))
            {
                if (this.SmtpSession.IsAuthenticated)
                {
                    response = SmtpResponse.AlreadyAuthenticated;
                }
                else
                {
                    response = await GetAuthResponse();
                }
            }
            await SmtpSession.SendResponseAsync(response);
        }

        private async Task<string> GetAuthResponse()
        {
            string response;
            var saslMechanism = SmtpSession.SaslFactory.GetSaslMechanism(this.Arguments.ElementAtOrDefault(0).Key);

            if (saslMechanism == null)
            {
                return SmtpResponse.AuthNotSupported;
            }

            try
            {
                if (saslMechanism.CanSendInitialResponse && this.Arguments.Count == 2)
                {
                    saslMechanism.ParseResponse(this.Arguments.ElementAtOrDefault(1).Key);
                }

                // Begin the Sasl challenge-response.
                while (!saslMechanism.IsCompleted)
                {
                    await this.SmtpSession.SendResponseAsync(saslMechanism.GetChallenge());
                    saslMechanism.ParseResponse((await this.SmtpSession.ListenRequestAsync()).FirstOrDefault());
                }

                this.SmtpSession.IsAuthenticated = saslMechanism.IsAuthenticated;

                if (saslMechanism.IsAuthenticated)
                {
                    this.SmtpSession.SmtpCommands.Add(this);
                    this.SmtpSession.SmtpStateTable.User = saslMechanism.User;
                    this.SmtpSession.SmtpStateTable.SessionState = SessionState.MailNeeded;
                    response = SmtpResponse.AuthOk;
                }
                else
                {
                    response = SmtpResponse.AuthCredInvalid;
                }

                return response;
            }
            catch (SaslException)
            {
                return SmtpResponse.AuthCredInvalid;
            }
            catch (Exception)
            {
                return SmtpResponse.AuthCredInvalid;
            }
        }
    }
}