using MimeKit;
using System.IO;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Common.Interfaces
{
    public interface IMessageData
    {
        //Stream InternalStream { get; }
        void DeleteMessageData();
        //void Dispose();
        MimeMessage MimeMesage { get; }
        Task<Stream> GetMessagStreamAsync();
        Task ReadFromAsync(Stream stream);
        Task WriteToAsync(Stream stream);
        Task SaveChangesAsync();
    }
}