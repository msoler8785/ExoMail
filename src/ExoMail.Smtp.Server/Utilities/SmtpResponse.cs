namespace ExoMail.Smtp.Server.Utilities
{
    public static class SmtpResponse
    {
        //TODO: Finish converting to ENHANCEDSTATUSCODES
        public static string AlreadyAuthenticated => "503 5.5.2 Already authenticated";

        public static string Announcment => "220 {0} Welcome to the ExoMail ESMTP Server";
        public static string ArgumentUnrecognized => "501 5.5.4 Syntax error in parameters or arguments";
        public static string AuthAborted => "501 5.0.0 Authentication Aborted";
        public static string AuthCredInvalid => "535 5.7.8 Authentication credentials invalid";
        public static string AuthEncryptRequired => "538 5.7.11 Encryption required for requested authentication mechanism";
        public static string AuthExchTooLong => "500 5.5.6 Authentication Exchange line is too long";
        public static string AuthLoginPassword => "334 UGFzc3dvcmQ6";
        public static string AuthLoginUserName => "334 VXNlcm5hbWU6";
        public static string AuthNotSupported => "504 5.5.4 Authentication mechanism not supported";
        public static string AuthOk => "235 2.7.0 Authentication Succeeded";
        public static string AuthPasswordTransition => "432 4.7.12 A password transition is needed";
        public static string AuthRequired => "530 5.7.0 Authentication required";
        public static string AuthStart => "334 Start Authentication";
        public static string AuthTempFailure => "454 4.7.0 Temporary authentication failure";
        public static string AuthWeak => "534 5.7.9 Authentication mechanism is too weak";
        public static string BadCommand => "503 5.5.1 Bad sequence of commands";
        public static string CannotVrfy => "252 2.1.5 Cannot VRFY user";
        public static string Closing => "221 2.0.0 {0} Service closing transmission channel";
        public static string CommandNotImplemented => "502 5.5.1 Command not implemented";
        public static string CommandUnrecognized => "500 5.5.1 Syntax error, command unrecognized";
        public static string ExceededStorage => "552 5.2.2 Requested mail action aborted: exceeded storage allocation";
        public static string Hello => "250 {0} Hello! {1}";
        public static string Help => "211 DATA HELO EHLO AUTH MAIL NOOP QUIT RCPT RSET";
        public static string InsufficientStorage => "452 4.3.1 Requested action not taken: insufficient system storage";
        public static string InvalidDomainName => "501 5.1.8 Invalid Domain Name";
        public static string InvalidRecipient => "550 5.5.2 Invalid syntax. Syntax should be RCPT TO:<userdomain>";
        public static string InvalidSenderName => "550 5.5.2 Invalid syntax. Syntax should be MAIL FROM:<userdomain>";
        public static string LocalError => "451 4.3.0 Requested action aborted: local error in processing";
        public static string MailboxFull => "452 4.2.2 The recipients mailbox is full";
        public static string MailboxUnavailable => "550 5.1.1 Requested action not taken: mailbox unavailable";
        public static string NameNotAllowed => "553 Requested action not taken: mailbox name not allowed";
        public static string OK => "250 2.0.0 OK";
        public static string ParameterNotImplemented => "504 5.5.4 Command parameter not implemented";
        public static string Queued => "250 2.6.0 Queued for delivery with MessageId <{0}>";
        public static string RecipientOK => "250 2.1.5 Recipient <{0}> OK";
        public static string RecipientSizeExceeded => "552 5.2.3 Message exceeds maximum message size of {0}MB for recipient {1}";
        public static string Resetting => "250 2.0.0 Resetting";
        public static string SenderAlreadySpecified => "503 5.5.2 Sender already specified";
        public static string SenderAnd8BitOK => "250 2.1.0 Sender <{0}> OK and 8BITMIME OK";
        public static string SenderAndRecipientFirst => "503 5.5.1 Must specify recipient and sender first";
        public static string SenderFirst => "503 5.5.1 Must specify sender first";
        public static string SenderOK => "250 2.1.0 Sender <{0}> OK";
        public static string SendHelloFirst => "503 5.5.2 Send hello first";
        public static string ServiceNotAvailable => "421 4.3.0 {0} Service not available, closing transmission channel";
        public static string StartInput => "354 Start mail input; end with <CRLF>.<CRLF>";
        public static string StartTls => "220 OK STARTTLS Go ahead";
        public static string StartTlsFirst => "530 5.7.0 Must issue a STARTTLS command first";
        public static string SystemSizeExceeded => "552 5.3.4 Message exceeds maximum message size of {0}MB for System";
        public static string Timeout => "451 4.7.0 Timeout waiting for client input";
        public static string TransactionFailed => "554 5.3.0 Transaction failed";
        public static string UnableToRelay => "550 5.7.1 Unable to relay";
        public static string MessageExpired => "550 5.4.7 Message expired in queue.";
        public static string SystemShuttingDown => "450 4.3.0 System is shutting down; please try again later";
    }
}