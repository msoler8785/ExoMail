using ExoMail.Smtp.Models;
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

        void Save(Stream stream, ReceivedHeader sessionMessage);
    }
}
