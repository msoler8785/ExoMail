using ExoMail.Smtp.Server.Protocol;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Interfaces
{
    public interface IMessageStore
    {
        //string FolderPath { get; set; }
        //string FileName { get; set; }
        //string FilePath { get; }
        //string MessageId { get; set; }
        //IMessageStore Save(Stream stream);
        //IMessageStore WithFolderPath(string directory);
        Task<string> Save(SmtpSession smtpSession, Stream message);
    }
}