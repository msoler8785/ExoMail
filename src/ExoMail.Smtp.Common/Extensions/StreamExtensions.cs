using ExoMail.Smtp.Common.Services;
using ExoMail.Smtp.Common.Utilities;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Common.Extensions
{
    public static class StreamExtensions
    {
        private static MD5 _md5 = MD5.Create();

        private static readonly byte[] _eolSeq = Encoding.ASCII.GetBytes("\r\n");

        private static bool IsEnd(int length, byte[] buffer, int offset, int count, byte[] termSeq)
        {
            if (buffer.Length < length)
                return false;

            bool eof = false;
            int cursor = length - termSeq.Length;

            for (int i = 0; i < termSeq.Length; i++)
            {
                eof = buffer[cursor + i] == termSeq[i];

                if (!eof)
                    break;
            }

            return eof;
        }

        public static async Task<bool> ReadAsync(this Stream stream, byte[] buffer, int offset, int count, byte[] termSeq)
        {
            int length = await stream.ReadAsync(buffer, offset, count);

            return IsEnd(length, buffer, offset, count, termSeq);
        }

        public static bool Read(this Stream stream, byte[] buffer, int offset, int count, byte[] termSeq)
        {
            int length = stream.Read(buffer, offset, count);

            return IsEnd(length, buffer, offset, count, termSeq);
        }

        public static async Task<string> ReadLine(this Stream stream)
        {
            byte[] buffer = new byte[512];
            int bytesRead = 0;
            int offset = 0;
            bool eol = false;

            using (var ms = BufferManager.GetStream())
            {
                do
                {
                    bytesRead = await stream.ReadAsync(buffer, offset, 1);
                    await ms.WriteAsync(buffer, offset, bytesRead);

                    offset += bytesRead;

                    if (offset + _eolSeq.Length == buffer.Length)
                        offset = 0;

                    for (int i = 0; i < buffer.Length; i++)
                    {
                        eol = buffer[i] == _eolSeq[0] &&
                           buffer[i + 1] == _eolSeq[1];

                        if (eol)
                            break;
                    }
                }
                while (!eol);

                string line = Encoding.ASCII.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                return line;
            }
        }

        /// <summary>
        /// Compares two stream to see if they are identical.
        /// </summary>
        /// <param name="sstream">this Stream</param>
        /// <param name="stream">The stream to compare</param>
        /// <returns>True if the streams are identicle.</returns>
        public static bool CompareTo(this Stream sstream, Stream stream)
        {
            if (!sstream.CanSeek | !stream.CanSeek)
                throw new ArgumentException("Cannot compare non seekable streams.");

            long sPos = sstream.Position;
            long dPos = stream.Position;

            byte[] shash = _md5.ComputeHash(sstream);
            byte[] dhash = _md5.ComputeHash(stream);

            bool isEqual = NativeMethods.MemCmp(shash, dhash);

            sstream.Position = sPos;
            stream.Position = dPos;

            return isEqual;
        }
    }
}