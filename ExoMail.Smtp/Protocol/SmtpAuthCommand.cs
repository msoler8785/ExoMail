using ExoMail.Smtp.Authentication;
using ExoMail.Smtp.Enums;
using ExoMail.Smtp.Exceptions;
using ExoMail.Smtp.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ExoMail.Smtp.Protocol
{
    public sealed class SmtpAuthCommand : SmtpCommandBase
    {
        public SmtpAuthCommand(string command, List<string> arguments)
        {
            Command = command;
            Arguments = arguments;
        }

        public override bool ValidateArgs(out string argumentsResponse)
        {
            argumentsResponse = String.Empty;
            bool result = this.Arguments.Count > 0 && this.Arguments.Count <= 2;

            if (!result)
                argumentsResponse = SmtpResponse.ArgumentUnrecognized;

            return result;
        }

        public override async Task<string> GetResponseAsync()
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
                    switch (this.SmtpSession.SessionState)
                    {
                        case SessionState.EhloNeeded:
                            response = SmtpResponse.SendHelloFirst;
                            break;

                        case SessionState.StartTlsNeeded:
                            response = SmtpResponse.StartTlsFirst;
                            break;

                        case SessionState.AuthNeeded:
                        case SessionState.MailNeeded:
                            response = await GetAuthResponse();
                            break;

                        default:
                            response = SmtpResponse.BadCommand;
                            break;
                    }
                }
            }
            return response;
        }

        private async Task<string> GetAuthResponse()
        {
            string response;
            var saslMechanism = SaslFactory.GetSaslMechanism(this.Arguments[0]);

            if (saslMechanism == null)
            {
                return SmtpResponse.AuthNotSupported;
            }

            try
            {
                if (saslMechanism.CanSendInitialResponse && this.Arguments.Count == 2)
                {
                    saslMechanism.ParseResponse(this.Arguments[1]);
                }

                // Begin the Sasl challenge-response.
                while (!saslMechanism.IsCompleted)
                {
                    await this.SmtpSession.SendResponseAsync(saslMechanism.GetChallenge());
                    saslMechanism.ParseResponse(await this.SmtpSession.ListenRequestAsync());
                }

                this.SmtpSession.IsAuthenticated = saslMechanism.IsAuthenticated;

                if (saslMechanism.IsAuthenticated)
                {
                    this.IsValid = true;
                    this.SmtpSession.SmtpCommands.Add(this);
                    this.SmtpSession.SessionState = SessionState.MailNeeded;
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