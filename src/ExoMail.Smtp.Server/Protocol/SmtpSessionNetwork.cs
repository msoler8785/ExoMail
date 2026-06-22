using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;

namespace ExoMail.Smtp.Server.Protocol
{
    public class SmtpSessionNetwork
    {
        public TcpClient TcpClient { get; set; }

        public IPAddress ClientIpAddress
        {
            get { return this.RemoteEndPoint.Address; }
        }

        public IPAddress ServerIpAdderss
        {
            get { return this.LocalEndPoint.Address; }
        }

        public SslStream SslStream { get; internal set; }

        /// <summary>
        /// The network stream for this tcp client.
        /// </summary>
        public NetworkStream NetworkStream { get { return this.TcpClient.GetStream(); } }

        public bool IsEncrypted
        {
            get
            {
                bool isEncrypted =
                    this.SslStream == null ?
                    false :
                    this.SslStream.IsEncrypted;

                return isEncrypted;
            }
        }

        public Stream Stream
        {
            get
            {
                if (this.IsEncrypted)
                    return this.SslStream;

                return this.NetworkStream;
            }
        }

        public string RemoteDomainName { get; set; }

        /// <summary>
        /// The local network endpoint for this session.
        /// </summary>
        public IPEndPoint LocalEndPoint
        {
            get
            {
                return (IPEndPoint)this.TcpClient.Client.LocalEndPoint ?? new IPEndPoint(IPAddress.Loopback, 0);
            }
        }

        /// <summary>
        /// The remote network endpoint for the client.
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get
            {
                return (IPEndPoint)this.TcpClient.Client.RemoteEndPoint ?? new IPEndPoint(IPAddress.Loopback, 0);
            }
        }

        private string _remotePtr;
        public string RemotePtr
        {
            get
            {
                if (string.IsNullOrEmpty(_remotePtr))
                    _remotePtr = Dns.GetHostEntry(this.RemoteEndPoint.Address).HostName;

                return _remotePtr;
            }
        }

        public SmtpSessionNetwork(TcpClient tcpClient)
        {
            this.TcpClient = tcpClient;
        }
    }
}