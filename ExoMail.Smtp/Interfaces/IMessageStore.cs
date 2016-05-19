using ExoMail.Smtp.Models;
using ExoMail.Smtp.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Interfaces
{
    public interface IMessageStore
    {
        string FolderPath { get; set; }
        string FileName { get; set; }
        string FilePath { get; }
        string MessageId { get; set; }
        IMessageStore Save(Stream stream, ReceivedHeader receiveHeader);
        IMessageStore WithFolderPath(string directory);
    }
}
