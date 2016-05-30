using ARSoft.Tools.Net;
using ARSoft.Tools.Net.Dns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Protocol
{
    public class SmtpSessionNetwork
    {
        public TcpClient TcpClient { get; set; }

        private Task<string> _ptrRecord;
        public Task<string> PtrRecordAsync
        {
            get
            {
                return _ptrRecord = _ptrRecord ?? GetPtrRecordAsync();
            }
        }

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
        public DomainName RemoteDomainName { get; set; }

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

        private async Task<string> GetPtrRecordAsync()
        {
            DomainName domain = await new DnsStubResolver().ResolvePtrAsync(this.ClientIpAddress);

            string ptrRecord = 
                domain == null ? 
                this.ClientIpAddress.ToString() : 
                domain.ToString().TrimEnd('.');

            return ptrRecord;
        }

        public SmtpSessionNetwork(TcpClient tcpClient)
        {
            this.TcpClient = tcpClient;
        }

    }
}
