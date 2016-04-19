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
        //SessionMessage SessionMessage { get; set; }
        //List<LocalRecipientCollection> MailRecipients { get; set; }
        void Save(Stream stream, ReceivedHeader receiveHeader);
        IMessageStore WithFolderPath(string directory);
    }
}
