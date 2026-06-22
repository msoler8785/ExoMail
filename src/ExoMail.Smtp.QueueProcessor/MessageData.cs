using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Common.Services;
using MimeKit;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Smtp.QueueProcessor
{
    public class MessageData : IMessageData
    {
        private bool _isDisposed;
        //private Stream _stream;
        private readonly string _messagePath;
        private readonly SemaphoreSlim _semaphore;
        private MimeMessage _mimeMessage;
        public MimeMessage MimeMesage => _mimeMessage;

        public MessageData(string messagePath)
        {
            //_stream = BufferManager.GetStream();
            _messagePath = messagePath;
            _semaphore = new SemaphoreSlim(1);
            _isDisposed = false;
        }

        public void Dispose()
        {
            lock (_semaphore)
            {
                if (!_isDisposed)
                {
                    _isDisposed = true;
                    _semaphore.Dispose();
                }
            }
        }

        public void DeleteMessageData()
        {
            try
            {
                _semaphore.Wait();

                if (File.Exists(_messagePath))
                    File.Delete(_messagePath);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Stream> GetMessagStreamAsync()
        {
            try
            {
                await _semaphore.WaitAsync();

                if (_mimeMessage == null)
                {
                    _mimeMessage = MimeMessage.Load(_messagePath);
                }
              
                var stream = BufferManager.GetStream();

                _mimeMessage.WriteTo(stream);
                //if (_stream.Length == 0)
                //{
                //    using (var fs = new FileStream(_messagePath, FileMode.Open, FileAccess.Read))
                //    {
                //        await fs.CopyToAsync(_stream);
                //    }
                //}

                //_stream.Position = 0;
                //await _stream.CopyToAsync(stream);
                //_stream.Position = 0;
                stream.Position = 0;

                return stream;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task ReadFromAsync(Stream stream)
        {
            try
            {
                await _semaphore.WaitAsync();
                stream.Position = 0;
                _mimeMessage = MimeMessage.Load(stream);


                //using (var fs = new FileStream(_messagePath, FileMode.Create))
                //{
                //    stream.Position = 0;
                //    await stream.CopyToAsync(fs);
                //    stream.Position = 0;
                //}

                //_stream.Position = 0;
                //await stream.CopyToAsync(_stream);
                //_stream.SetLength(stream.Length);
                //_stream.Position = 0;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task WriteToAsync(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            try
            {
                await _semaphore.WaitAsync();

                if (_mimeMessage == null)
                {
                    _mimeMessage = MimeMessage.Load(_messagePath);
                }

                //using (var ms = await GetMessagStream())
                //{
                //    await ms.CopyToAsync(stream);
                //}
                _mimeMessage.WriteTo(stream);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task SaveChangesAsync()
        {
            if (_mimeMessage == null)
                throw new ArgumentNullException("_mimeMessage");

            try
            {
                await _semaphore.WaitAsync();
                using (var fs = new FileStream(_messagePath, FileMode.Create))
                {
                    _mimeMessage.WriteTo(fs);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        //private async Task LoadMessageAsync()
        //{
        //    await Task.Run(() =>
        //    {
        //        using (var fs = new FileStream(_messagePath, FileMode.Open))
        //        {
        //            _mimeMessage = MimeMessage.Load(fs);
        //        }
        //    });
        //}
    }
}