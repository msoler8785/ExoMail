using ExoMail.Smtp.Authentication;
using ExoMail.Smtp.Enums;
using ExoMail.Smtp.Interfaces;
using ExoMail.Smtp.Utilities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
    public sealed class SmtpRcptCommand : SmtpCommandBase
    {
        private IUserIdentity _user
        {
            get
            {
                return UserManager.GetUserManager.FindByEmailAddress(this._recipientAddress);
            }
        }

        private bool _isMessageSizeOk
        {
            get
            {
                if (this._user == null)
                {
                    return this._messageSize <= this.SmtpSession.ServerConfig.MaxMessageSize;
                }
                else
                {
                    return this._messageSize <= this.SmtpSession.ServerConfig.MaxMessageSize &&
                        this._messageSize <= this._user.MaxMessageSize;
                }
            }
        }

        private int _messageSize { get; set; }
        private string _recipientAddress { get; set; }
        private string _recipientDomain { get; set; }
        private bool _isValidRecipient
        {
            get
            {
                return UserManager.GetUserManager.IsValidRecipient(_recipientAddress);
            }
        }

        public SmtpRcptCommand(string command, List<string> arguments)
        {
            Command = command;
            Arguments = arguments;
        }



        public override bool ValidateArgs(out string argumentsResponse)
        {
            argumentsResponse = String.Empty;
            bool result = this.Arguments.Count >= 1 && this.Arguments.Count <= 2;

            if (!result)
                argumentsResponse = SmtpResponse.ArgumentUnrecognized;

            return result;
        }


        public override async Task<string> GetResponseAsync()
        {
            return await Task.Run(() => GetResponse());
        }

        private string GetResponse()
        {
            string response;

            if (ValidateArgs(out response))
            {
                switch (this.SmtpSession.SessionState)
                {
                    case SessionState.EhloNeeded:
                        response = SmtpResponse.SendHelloFirst;
                        break;

                    case SessionState.StartTlsNeeded:
                        response = SmtpResponse.StartTlsFirst;
                        break;

                    case SessionState.MailNeeded:
                        response = SmtpResponse.SenderFirst;
                        break;

                    case SessionState.RcptNeeded:
                    case SessionState.DataNeeded:
                        response = GetRcptResponse();
                        break;

                    default:
                        response = SmtpResponse.BadCommand;
                        break;
                }
            }

            return response;
        }

        private string GetRcptResponse()
        {
            // Regex to capture the recipient Argument
            var emailRegex = Regex.Match(this.Arguments[0], @"TO:<(.*@(.*\..*))>", RegexOptions.IgnoreCase);
            var validRecipientFormat = emailRegex.Success;
            int messageSize = 0;

            if (validRecipientFormat)
            {
                this._recipientAddress = emailRegex.Groups[1].Value;
                this._recipientDomain = emailRegex.Groups[2].Value;

                if (this.Arguments.Count == 2)
                {
                    if (TryParseMessageSize(out messageSize))
                    {
                        this._messageSize = messageSize;
                    }
                    else
                    {
                        return SmtpResponse.ArgumentUnrecognized;
                    }
                }

                if (this._isValidRecipient)
                {
                    if (this._isMessageSizeOk)
                    {
                        return SetValidRecipient();
                    }
                    else
                    {
                        return SmtpResponse.RecipientSizeExceeded;
                    }
                }
                else if (this.SmtpSession.ServerConfig.IsAuthRelayAllowed)
                {
                    return this.SmtpSession.IsAuthenticated ?
                         SetValidRecipient() : SmtpResponse.UnableToRelay;
                }
                else
                {
                    return SmtpResponse.MailboxUnavailable;
                }
            }
            else
            {
                return SmtpResponse.InvalidRecipient;
            }
        }

        private bool TryParseMessageSize(out int messageSize)
        {
            var sizeRegex = Regex.Match(this.Arguments[1], @"(SIZE)=(\d+)", RegexOptions.IgnoreCase);
            var isValidSizeArg = sizeRegex.Success;
            return int.TryParse(sizeRegex.Groups[2].Value, out messageSize) && isValidSizeArg;
        }

        private string SetValidRecipient()
        {
            this.IsValid = true;
            this.SmtpSession.SessionState = SessionState.DataNeeded;
            this.SmtpSession.SmtpCommands.Add(this);
            this.SmtpSession.MessageEnvelope.AddRecipient(this._recipientAddress, this._recipientDomain);
            return String.Format(SmtpResponse.RecipientOK, this._recipientAddress);
        }
    }
}