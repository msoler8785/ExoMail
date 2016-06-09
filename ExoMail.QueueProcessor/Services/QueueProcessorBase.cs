using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.QueueProcessor.Services
{
    public abstract class QueueProcessorBase
    {
        public string WatchFolder { get; set; }

        public int ProcessingInterval { get; set; } = 500;

        public FileSystemWatcher Watcher { get; set; }

        public BlockingCollection<string> Envelopes = new BlockingCollection<string>();

        public virtual int ProcessorTasks { get; set; } = 8;

        public QueueProcessorBase()
        {
        }

        public void Start()
        {
            CreateDirectoryIfNeeded(this.WatchFolder);

            this.Watcher = new FileSystemWatcher(this.WatchFolder);
            this.Watcher.Created += OnCreated;
            this.Watcher.Filter = "*.eml.env";
            //this.Watcher.InternalBufferSize = ByteSizeHelper.FromMegaBytes(1);
            this.Watcher.EnableRaisingEvents = true;

            ReloadUnprocessedEnvelopes();

            for (int i = 0; i < ProcessorTasks; i++)
            {
               ProcessEnvelopeQueue();
            }
        }

        internal abstract Task ProcessEnvelopeQueue();

        public void WaitForFile(String fileName)
        {
            bool fileReady = false;

            while (!fileReady)
            {
                FileStream inputStream = null;
                // If the file can be opened for exclusive access it means that the file
                // is no longer locked by another process.
                try
                {
                    inputStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None);

                    fileReady = inputStream.Length > 0;
                }
                catch (IOException)
                {
                    fileReady = false;
                }
                finally
                {
                    if (inputStream != null)
                        inputStream.Close();
                }

                if (fileReady) break;

                Thread.Sleep(100);
            }
        }

        public Task ReloadUnprocessedEnvelopes()
        {
            return Task.Run(() =>
            {
                var envelopes = Directory.EnumerateFiles(this.WatchFolder, "*.eml.env");

                foreach (var item in envelopes)
                {
                    Envelopes.Add(item);
                }
            });
        }

        public void OnCreated(object sender, FileSystemEventArgs e)
        {
            Envelopes.Add(e.FullPath);
        }

        public void CreateDirectoryIfNeeded(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}