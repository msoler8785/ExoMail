using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.IO
{
    public sealed class InboundQueueProcessor
    {
        private FileSystemWatcher _watcher { get; set; }
        public string FolderPath { get; private set; }

        public InboundQueueProcessor()
        {
            _watcher = new FileSystemWatcher();
        }

        public static InboundQueueProcessor Create(string folderPath)
        {
            var processor = new InboundQueueProcessor() { FolderPath = folderPath };
            processor._watcher.Path = folderPath;
            processor._watcher.Created += new FileSystemEventHandler(OnCreated);
            processor._watcher.EnableRaisingEvents = true;

            return processor;
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            using (var stream = new FileStream(e.FullPath, FileMode.Open))
            {
                var mimeMessage = new MimeParser(stream).ParseMessage();
            }
        }
    }
}
