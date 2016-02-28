using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExoMail.Smtp.Network;

namespace ExoMail.Smtp.Utilities
{
    public static class SmtpResponse
    {
        //TODO: Convert to ENHANCEDSTATUSCODES

        //public static string HostName                   { get { return ExoMail.Smtp.Network.SmtpServer.Core.ServerConfig.HostName; } }
        public static string Announcment                { get { return "220 {0} Welcome to the ExoMail ESMTP Server"; } }
        public static string Help                       { get { return "211 DATA HELO EHLO AUTH MAIL NOOP QUIT RCPT RSET"; } }
        public static string Closing                    { get { return "221 {0} Service closing transmission channel"; } }
        public static string Queued                     { get { return "250 Queued for delivery"; } }
        public static string OK                         { get { return "250 OK"; } }
        public static string Hello                      { get { return "250 {0} Hello! {1}"; } }
        public static string StartInput                 { get { return "354 Start mail input; end with <CRLF>.<CRLF>"; } }
        public static string ServiceNotAvailable        { get { return "421 {0} Service not available, closing transmission channel"; } }
        public static string LocalError                 { get { return "451 Requested action aborted: local error in processing"; } }
        public static string InsufficientStorage        { get { return "452 Requested action not taken: insufficient system storage"; } }
        public static string CommandUnrecognized        { get { return "500 Syntax error, command unrecognized"; } }
        public static string ArgumentUnrecognized       { get { return "501 Syntax error in parameters or arguments"; } }
        public static string InvalidDomainName          { get { return "501 Invalid Domain Name"; } }
        public static string CommandNotImplemented      { get { return "502 Command not implemented"; } }
        public static string BadCommand                 { get { return "503 Bad sequence of commands"; } }
        public static string SenderFirst                { get { return "503 Must specify sender first"; } }
        public static string SenderAndRecipientFirst    { get { return "503 Must specify recipient and sender first"; } }
        public static string ParameterNotImplemented    { get { return "504 Command parameter not implemented"; } }
        public static string StartTlsFirst              { get { return "530 Must issue a STARTTLS command first"; } }
        public static string MailboxUnavailable         { get { return "550 Requested action not taken: mailbox unavailable"; } }
        public static string InvalidSenderName          { get { return "550 Invalid syntax. Syntax should be MAIL FROM:<userdomain>"; } }
        public static string InvalidRecipient           { get { return "550 Invalid syntax. Syntax should be RCPT TO:<userdomain>"; } }
        public static string ExceededStorage            { get { return "552 Requested mail action aborted: exceeded storage allocation"; } }
        public static string MessageSizeExceeded        { get { return "552 Message exceeds maximum message size of {0}MB"; } }
        public static string NameNotAllowed             { get { return "553 Requested action not taken: mailbox name not allowed"; } }
        public static string TransactionFailed          { get { return "554 Transaction failed - "; } }
        public static string StartTls                   { get { return "220 OK STARTTLS Go ahead"; } }
        public static string AuthOk                		{ get { return "235 2.7.0  Authentication Succeeded"; } }
        public static string AuthLoginUserName          { get { return "334 VXNlcm5hbWU6"; } }
        public static string AuthLoginPassword          { get { return "334 UGFzc3dvcmQ6"; } }
        public static string AuthPasswordTransition     { get { return "432 4.7.12  A password transition is needed"; } }
        public static string AuthTempFailure            { get { return "454 4.7.0  Temporary authentication failure"; } }
        public static string AuthWeak                	{ get { return "534 5.7.9  Authentication mechanism is too weak"; } }
        public static string AuthCredInvalid            { get { return "535 5.7.8  Authentication credentials invalid"; } }
        public static string AuthExchTooLong            { get { return "500 5.5.6  Authentication Exchange line is too long"; } }
        public static string AuthAborted                { get { return "501 5.0.0  Authentication Aborted"; } }
        public static string AuthRequired               { get { return "530 5.7.0  Authentication required"; } }
        public static string AuthEncryptRequired        { get { return "538 5.7.11  Encryption required for requested authentication mechanism"; } }

        public static string Ehlo
        {
            get
            {
                return      "250-{0} Hello {1}\r\n" +
                            "250-SIZE {2}\r\n" +
                            "250-AUTH {3}\r\n" +
                            "250-STARTTLS\r\n" +
                            "250 HELP";
            }
        }
    }
}
