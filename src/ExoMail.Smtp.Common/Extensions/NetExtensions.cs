using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Common.Extensions
{
    public static class NetExtensions
    {
        public static void Close(this TcpClient tcpClient)
        {
            try
            {
                tcpClient.Dispose();
            }
            catch (ObjectDisposedException)
            {
            }
        }

        public static async Task<TcpClient> AcceptTcpClientAsync(this TcpListener listener, CancellationToken token)
        {
            try
            {
                return await listener.AcceptTcpClientAsync();
            }
            catch (Exception ex) when (token.IsCancellationRequested)
            {
                throw new OperationCanceledException("Cancellation was requested while awaiting TCP client connection.", ex);
            }
        }
    }
}