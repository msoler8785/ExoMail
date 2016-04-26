using ExoMail.Smtp.Interfaces;
using ExoMail.Smtp.Models;
using System;
using System.IO;

namespace ExoMail.Smtp.Server.IO
{
    /// <summary>
    /// An example implementation of storing messages on the file system.
    /// </summary>
    public class FileMessageStore : IMessageStore
    {
        public string FolderPath { get; set; }
        public string FileName { get; set; }
        public string MessageId { get; set; }
        public string FilePath { get { return Path.Combine(this.FolderPath, this.FileName); } }

        /// <summary>
        /// Create a new FileMessageStore instance.
        /// </summary>
        public static FileMessageStore Create { get { return new FileMessageStore(); } }

        private FileMessageStore()
        {
        }

        /// <summary>
        /// The path to the folder to store this message.
        /// </summary>
        /// <param name="path">An absolute folder path.</param>
        /// <returns>this</returns>
        public IMessageStore WithFolderPath(string path)
        {
            this.FolderPath = path;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return this;
        }

        /// <summary>
        /// The file name to use for this message.
        /// </summary>
        /// <param name="fileName">A name for the file including the extension.</param>
        /// <returns>this</returns>
        public FileMessageStore WithFileName(string fileName)
        {
            this.FileName = fileName;
            return this;
        }

        /// <summary>
        /// Saves the message to the specified folder.
        /// </summary>
        public IMessageStore Save(Stream stream, ReceivedHeader receivedHeader)
        {
            this.MessageId = Guid.NewGuid().ToString();
            this.FolderPath = this.FolderPath ?? AppDomain.CurrentDomain.BaseDirectory;
            this.FileName = this.MessageId + ".eml";

            string path = Path.Combine(this.FolderPath, this.FileName);

            using (FileStream fileStream = new FileStream(path, FileMode.Append))
            {
                receivedHeader.GetReceivedHeaders().CopyTo(fileStream);
                stream.CopyTo(fileStream);
            }
            return this;
        }
    }
}