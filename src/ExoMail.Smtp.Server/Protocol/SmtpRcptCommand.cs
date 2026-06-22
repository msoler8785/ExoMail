using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Common.Models;
using ExoMail.Smtp.Server.Enums;
using ExoMail.Smtp.Server.Utilities;
using System;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Protocol
{
    public sealed class SmtpRcptCommand : SmtpCommandBase
    {
        private IUserIdentity User { get; set; }
        private MailAddress Recipient { get; set; }

        private bool MessageSizeValid
        {
            get
            {
                bool serverSize = this.MessageSize <= this.SmtpSession._serverConfig.MaxMessageSize;

                if (User != null)
                {
                    bool userSize = this.MessageSize <= this.User.MaxMessageSize;

                    return userSize && serverSize;
                }

                return serverSize;
            }
        }

        private int MessageSize { get; set; }

        public override SessionState AllowedStates
        {
            get
            {
                return SessionState.RcptNeeded | SessionState.DataNeeded;
            }
        }

        public SmtpRcptCommand()
            : base()
        {
            MessageSize = 0;
        }

        public override bool ValidateArgs(out string argumentsResponse)
        {
            argumentsResponse = String.Empty;
            bool result = this.Arguments.Count == 1;
            MailAddress recipient;
            bool authRelay = this.SmtpSession.IsAuthenticated && this.SmtpSession._serverConfig.IsAuthRelayAllowed;

            if (!result)
            {
                argumentsResponse = SmtpResponse.ArgumentUnrecognized;
            }
            else if (Arguments.ContainsKey("TO") && MailAddress.TryParse(Arguments["TO"], false, out recipient))
            {
                var user = SmtpSession._virtualServerRepository
                    .FindByEmailAddress(recipient.FullAddress);

                if (user == null && !authRelay)
                {
                    argumentsResponse = SmtpResponse.MailboxUnavailable;
                    result = false;
                }
                else if (!this.MessageSizeValid)
                {
                    argumentsResponse = SmtpResponse.RecipientSizeExceeded;
                    result = false;
                }
                else
                {
                    Recipient = recipient;
                    argumentsResponse = String.Format(SmtpResponse.RecipientOK, recipient.FullAddress);
                }
            }
            else
            {
                argumentsResponse = SmtpResponse.InvalidRecipient;
                result = false;
            }

            return result;
        }

        public override async Task ExecuteAsync()
        {
            string response;

            if (ValidateArgs(out response))
            {
                SmtpSession.SmtpStateTable.Recipients.Add(Recipient);
                this.SmtpSession.SmtpStateTable.SessionState = SessionState.DataNeeded;
            }

            await SmtpSession.SendResponseAsync(response);
        }

        //private string GetRcptResponse()
        //{
        //    MailAddress recipient;
        //    bool authRelay = this.SmtpSession.IsAuthenticated && this.SmtpSession.ServerConfig.IsAuthRelayAllowed;

        //    if (Arguments.ContainsKey("TO") && MailAddress.TryParse(Arguments["TO"], false, out recipient))
        //    {
        //        var user = UserManager.GetUserManager.FindByEmailAddress(recipient.FullAddress);

        //        if (user == null && !authRelay)
        //        {
        //            return SmtpResponse.MailboxUnavailable;
        //        }
        //        else if (!this.MessageSizeValid)
        //        {
        //            return SmtpResponse.RecipientSizeExceeded;
        //        }
        //        else
        //        {
        //            SmtpSession.SmtpStateTable.Recipients.Add(recipient);
        //            this.SmtpSession.SmtpStateTable.SessionState = SessionState.DataNeeded;
        //            return String.Format(SmtpResponse.RecipientOK, recipient.FullAddress);
        //        }
        //    }

        //    return SmtpResponse.InvalidRecipient;

        //}
    }
}