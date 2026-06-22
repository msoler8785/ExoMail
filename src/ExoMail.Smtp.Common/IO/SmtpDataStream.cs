using ExoMail.Smtp.Common.Services;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Logging;

namespace ExoMail.Smtp.Common.IO
{
    public class SmtpDataStream : Stream
    {
        private const byte NEWLINE = (byte)'\n';
        private const byte DOT = (byte)'.';
        private static byte[] _dataTerminator = Encoding.ASCII.GetBytes("\r\n.\r\n");
        private bool _disposed;
        private byte[] _readBuffer;
        private byte[] _clientBuffer;
        private readonly ILogger<SmtpDataStream> _logger;
        private Stream _innerStream;

        public MemoryStream MessageBuffer { get; private set; }
        public int TotalRead { get; private set; }

        public bool EndOfStream
        {
            get
            {
                bool eos = false;
                long position = MessageBuffer.Position;
                if (MessageBuffer.Length > _dataTerminator.Length)
                {
                    MessageBuffer.Seek(_readBuffer.Length * -1, SeekOrigin.End);
                    int bytesRead = MessageBuffer.Read(_readBuffer, 0, _readBuffer.Length);

                    if (bytesRead >= _dataTerminator.Length)
                    {
                        eos =
                            _readBuffer[0] == _dataTerminator[0] &&
                            _readBuffer[1] == _dataTerminator[1] &&
                            _readBuffer[2] == _dataTerminator[2] &&
                            _readBuffer[3] == _dataTerminator[3] &&
                            _readBuffer[4] == _dataTerminator[4];
                    }

                    MessageBuffer.Position = position;
                }
                return eos;
            }
        }

        public SmtpDataStream(Stream innerStream, ILoggerFactory loggerFactory)
        {
            _innerStream = innerStream;
            MessageBuffer = BufferManager.GetStream();
            _readBuffer = new byte[_dataTerminator.Length];
            _clientBuffer = new byte[8 * 1024];
            _logger = loggerFactory.CreateLogger<SmtpDataStream>();
        }

        public override bool CanRead => _innerStream.CanRead;
        public override bool CanSeek => _innerStream.CanSeek;
        public override bool CanWrite => _innerStream.CanWrite;
        public override long Length => _innerStream.Length;

        public override long Position
        {
            get
            {
                return _innerStream.Position;
            }
            set
            {
                _innerStream.Position = value;
            }
        }

        public override void Flush()
            => _innerStream.Flush();

        public async Task ReadClient(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            TotalRead += await ReadAsync(_clientBuffer, 0, _clientBuffer.Length, token);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (EndOfStream)
                return 0;

            //if (InnerBuffer.Length < buffer.Length)
            //{
            //    InnerBuffer = new byte[buffer.Length];
            //}
            //byte[] innerBuffer = new byte[buffer.Length];

            bool dot = false;

            int bytesRead = _innerStream.Read(buffer, 0, count);
            var innerBufferList = buffer.ToList();

            for (int i = 0; i < bytesRead; i++)
            {
                dot = innerBufferList[i] == NEWLINE &&
                    i + 1 < bytesRead &&
                    innerBufferList[i + 1] == DOT &&
                    innerBufferList[i + 2] == DOT;

                buffer[i] = innerBufferList[i];

                if (dot)
                {
                    innerBufferList.RemoveAt(i + 2);
                    bytesRead--;
                }
            }

            MessageBuffer.Write(buffer, 0, bytesRead);

            //#if DEBUG
            //            // TODO: Replace this with verbose logging interface.
            //            Console.Write(Encoding.ASCII.GetString(buffer, 0, bytesRead));
            //#endif

            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
            => _innerStream.Seek(offset, origin);

        public override void SetLength(long value)
            => _innerStream.SetLength(value);

        //public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        //{
        //    await Task.Run(() => this.Write(buffer, offset, count), cancellationToken);
        //}

        //public override async Task<int> ReadAsync(Byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        //{
        //    return await Task.Run(() => this.Read(buffer, offset, count), cancellationToken);
        //}

        public override void Write(byte[] buffer, int offset, int count)
        {
            var bufferList = buffer.ToList();
            bool dot = false;
            int dots = 0;

            for (int i = 0; i < count - 1; i++)
            {
                dot =
                    bufferList[i] == NEWLINE &&
                    bufferList[i + 1] == DOT;

                if (dot)
                {
                    i++;
                    dots++;
                    bufferList.Insert(i, DOT);
                }
            }

            _innerStream.Write(bufferList.ToArray(), offset, count + dots);
        }

        public async Task TerminateDataStream()
        {
            await _innerStream.WriteAsync(_dataTerminator, 0, _dataTerminator.Length);
        }

        //public override int Read(byte[] buffer, int offset, int count)
        //{
        //    int bytesRead = InnerStream.Read(buffer, offset, count);

        //    MemoryStream.Write(buffer, 0, bytesRead);

        //    return bytesRead;
        //}

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!_disposed)
            {
                MessageBuffer.Dispose();
                _disposed = true;
            }
        }

        ~SmtpDataStream()
        {
            Dispose(false);
        }
    }
}