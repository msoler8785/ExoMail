using ExoMail.Smtp.Authentication;
using ExoMail.Smtp.Interfaces;
using ExoMail.Smtp.Models;
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

        private InboundQueueProcessor()
        {
            _watcher = new FileSystemWatcher();
        }

        public static InboundQueueProcessor Create(string folderPath)
        {
            var fileFilter = "*.eml.que";
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var processor = new InboundQueueProcessor() { FolderPath = folderPath };
            processor._watcher.Path = folderPath;
            processor._watcher.EnableRaisingEvents = true;
            processor._watcher.Filter = fileFilter;
            processor._watcher.Created += new FileSystemEventHandler(ProcessFiles);
            //processor._watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.CreationTime;
            //processor._watcher.Changed += new FileSystemEventHandler(ProcessFiles);

            foreach (var item in Directory.EnumerateFiles(folderPath, fileFilter))
            {
                var args = new FileSystemEventArgs(WatcherChangeTypes.Changed, folderPath, Path.GetFileName(item));
                ProcessFiles(null, args);
            }

            return processor;
        }

        private static void ProcessFiles(object sender, FileSystemEventArgs e)
        {
            var agent = DeliveryAgent.Load(e.FullPath);
            foreach (var recipient in agent.Recipients)
            {
                var user = UserManager.GetUserManager.FindByEmailAddress(recipient);
                var recipientMailbox = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users", user.MailboxPath, "Inbox");

                if (!Directory.Exists(recipientMailbox))
                    Directory.CreateDirectory(recipientMailbox);

                var destination = Path.Combine(recipientMailbox, Path.GetFileName(agent.MessagePath));
                if(!File.Exists(destination))
                {
                    File.Copy(agent.MessagePath, destination);
                }
            }
            File.Delete(e.FullPath);
            File.Delete(agent.MessagePath);
        }
    }
}
