using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail
{
    public abstract class SmtpServerBase : IDisposable
    {
        private static List<TcpListener> TcpListeners { get; private set; }
        private static List<IPEndPoint> IPEndPoints { get; private set; }

        public SmtpServerBase()
        {

        }

        public void Start()
        {

        }

        public void Stop()
        {

        }
        ~SmtpServerBase()
        {

        }
        private bool _disposed;
        private readonly object _disposeLock = new object();

        /// <summary>
        /// Inheritable dispose method
        /// </summary>
        /// <param name="disposing">true, suppress GC finalizer call</param>
        protected virtual void Dispose(bool disposing)
        {
            lock (_disposeLock)
            {
                if (!_disposed)
                {
                    _disposed = true;
                    if (disposing) GC.SuppressFinalize(this);
                }
            }
        }
    }
}
