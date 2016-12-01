using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Utilities
{
    public static class SmtpResponse
    {
        //TODO: Finish converting to ENHANCEDSTATUSCODES
        public static string Announcment { get { return "220 {0} Welcome to the ExoMail ESMTP Server"; } }
        public static string Help { get { return "211 DATA HELO EHLO AUTH MAIL NOOP QUIT RCPT RSET"; } }
        public static string Closing { get { return "221 2.0.0 {0} Service closing transmission channel"; } }
        public static string Queued { get { return "250 Queued for delivery"; } }
        public static string OK { get { return "250 2.0.0 OK"; } }
        public static string SenderOK { get { return "250 2.1.0 Sender OK"; } }
        public static string RecipientOK { get { return "250 2.1.5 Recipient <{0}> OK"; } }
        public static string SenderAnd8BitOK { get { return "250 2.1.0 Sender OK and 8BITMIME OK"; } }
        public static string Resetting { get { return "250 2.0.0 Resetting"; } }
        public static string Hello { get { return "250 {0} Hello! {1}"; } }
        public static string CannotVrfy { get { return "252 2.1.5 Cannot VRFY user"; } }
        public static string StartInput { get { return "354 Start mail input; end with <CRLF>.<CRLF>"; } }
        public static string ServiceNotAvailable { get { return "421 4.3.0 {0} Service not available, closing transmission channel"; } }
        public static string LocalError { get { return "451 4.3.0 Requested action aborted: local error in processing"; } }
        public static string InsufficientStorage { get { return "452 4.3.1 Requested action not taken: insufficient system storage"; } }
        public static string CommandUnrecognized { get { return "500 5.5.1 Syntax error, command unrecognized"; } }
        public static string ArgumentUnrecognized { get { return "501 5.5.4 Syntax error in parameters or arguments"; } }
        public static string InvalidDomainName { get { return "501 5.1.8 Invalid Domain Name"; } }
        public static string CommandNotImplemented { get { return "502 5.5.1 Command not implemented"; } }
        public static string BadCommand { get { return "503 5.5.1 Bad sequence of commands"; } }
        public static string SenderFirst { get { return "503 5.5.1 Must specify sender first"; } }
        public static string SenderAndRecipientFirst { get { return "503 5.5.1 Must specify recipient and sender first"; } }
        public static string ParameterNotImplemented { get { return "504 5.5.4 Command parameter not implemented"; } }
        public static string StartTlsFirst { get { return "530 5.7.0 Must issue a STARTTLS command first"; } }
        public static string MailboxUnavailable { get { return "550 5.1.1 Requested action not taken: mailbox unavailable"; } }
        public static string InvalidSenderName { get { return "550 5.5.2 Invalid syntax. Syntax should be MAIL FROM:<userdomain>"; } }
        public static string InvalidRecipient { get { return "550 5.5.2 Invalid syntax. Syntax should be RCPT TO:<userdomain>"; } }
        public static string ExceededStorage { get { return "552 5.2.2 Requested mail action aborted: exceeded storage allocation"; } }
        public static string RecipientSizeExceeded { get { return "552 5.2.3 Message exceeds maximum message size of {0}MB for recipient {1}"; } }
        public static string SystemSizeExceeded { get { return "552 5.3.4 Message exceeds maximum message size of {0}MB for System"; } }
        public static string NameNotAllowed { get { return "553 Requested action not taken: mailbox name not allowed"; } }
        public static string TransactionFailed { get { return "554 5.3.0 Transaction failed"; } }
        public static string StartTls { get { return "220 OK STARTTLS Go ahead"; } }
        public static string AuthOk { get { return "235 2.7.0 Authentication Succeeded"; } }
        public static string AuthStart { get { return "334 Start Authentication"; } }
        public static string AuthLoginUserName { get { return "334 VXNlcm5hbWU6"; } }
        public static string AuthLoginPassword { get { return "334 UGFzc3dvcmQ6"; } }
        public static string MailboxFull { get { return "452 4.2.2 The recipients mailbox is full"; } }
        public static string AuthPasswordTransition { get { return "432 4.7.12 A password transition is needed"; } }
        public static string AuthTempFailure { get { return "454 4.7.0 Temporary authentication failure"; } }
        public static string AuthWeak { get { return "534 5.7.9 Authentication mechanism is too weak"; } }
        public static string AuthCredInvalid { get { return "535 5.7.8 Authentication credentials invalid"; } }
        public static string AuthExchTooLong { get { return "500 5.5.6 Authentication Exchange line is too long"; } }
        public static string AuthAborted { get { return "501 5.0.0 Authentication Aborted"; } }
        public static string SendHelloFirst { get { return "503 5.5.2 Send hello first"; } }
        public static string SenderAlreadySpecified { get { return "503 5.5.2 Sender already specified"; } }
        public static string AlreadyAuthenticated { get { return "503 5.5.2 Already authenticated"; } }
        public static string AuthNotSupported { get { return "504 5.5.4 Authentication mechanism not supported"; } }
        public static string AuthRequired { get { return "530 5.7.0 Authentication required"; } }
        public static string AuthEncryptRequired { get { return "538 5.7.11 Encryption required for requested authentication mechanism"; } }
        public static string UnableToRelay { get { return "550 5.7.1 Unable to relay"; } }
    }
}
