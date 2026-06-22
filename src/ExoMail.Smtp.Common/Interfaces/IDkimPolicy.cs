using MimeKit.Cryptography;

namespace ExoMail.Smtp.Common.Interfaces
{
    public interface IDkimPolicy
    {
        bool UseDkim { get; }
        string Domain { get; }
        string Selector { get; }
        string[] Headers { get; }
        DkimSigner DkimSigner { get; }
    }
}