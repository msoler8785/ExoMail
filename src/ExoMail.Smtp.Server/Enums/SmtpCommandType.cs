namespace ExoMail.Smtp.Server.Enums
{
    public enum SmtpCommandType
    {
        EHLO,
        HELO,
        MAIL,
        RCPT,
        BDAT,
        DATA,
        RSET,
        QUIT,
        NOOP,
        STARTTLS,
        AUTH,
        HELP,
        SAML,
        VRFY,
        TURN,
        INVALID
    }
}