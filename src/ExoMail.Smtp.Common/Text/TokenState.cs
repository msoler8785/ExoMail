namespace ExoMail.Smtp.Common.Text
{
    public enum TokenState
    {
        InitKey = 4,
        InitVal = 8,
        Quote = 16,
        Escaped = 32,
        Finished = 64,
        Delimeter = 128
    }
}