using ExoMail.Smtp.Interfaces;
using ExoMail.Smtp.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Models
{
    public class NetworkValidator : ISessionValidator
    {
        public List<IPNetwork> AllowedNetworks { get; set; }
        public List<IPNetwork> DeniedNetworks { get; set; }
        public SmtpSession SmtpSession { get; set; }
        public NetworkValidator()
        {
            this.AllowedNetworks = new List<IPNetwork>();
            this.DeniedNetworks = new List<IPNetwork>();
        }

        public bool IsValid()
        {
            var ipAddress = ((IPEndPoint)this.SmtpSession.TcpClient.Client.RemoteEndPoint).Address;

            if (this.SmtpSession.IsAuthenticated && this.SmtpSession.IsAuthenticatedRelayAllowed) 
            {
                return true;
            }
            else
            {
                bool isValid = 
                    this.AllowedNetworks.Any(x => IPNetwork.Contains(x, ipAddress)) ||
                    this.DeniedNetworks.Any(x => !IPNetwork.Contains(x, ipAddress));

                return isValid;
            }    
        }
    }
}
