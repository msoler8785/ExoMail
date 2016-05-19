using ExoMail.Smtp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExoMail.Smtp.Protocol;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;

namespace ExoMail.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var appStart = new AppStart();
            appStart.Start();

            Console.ReadLine();
        }
    }
    public class AppStart
    {
        public TcpListener TcpListener { get; set; }
        public void Start()
        {
            Task.Run(() => StartListeningAsync()).Wait();
        }

        public async Task StartListeningAsync()
        {
            var config = new MemoryConfig();
            var messageStore = new FileMessageStore();
            var userStore = new TestUserStore();

            this.TcpListener = new TcpListener(config.ServerIpBinding, config.Port);
            this.TcpListener.Start();
            TcpClient tcpClient;

            while (true)
            {
                tcpClient = await this.TcpListener.AcceptTcpClientAsync();
                CreateSession(tcpClient, config, messageStore, userStore);
            }
        }

        public void CreateSession(TcpClient tcpClient, IServerConfig config, IMessageStore messageStore, IUserStore userStore)
        {
            Task.Run(async () =>
            {
                var session = new SmtpSession()
                {
                    ServerConfig = config,
                    MessageStore = messageStore,
                    UserStore = userStore
                };

                await session.BeginSession(tcpClient);
            });
        }
    }
    public class FileMessageStore : IMessageStore
    {
        public async Task Save(MemoryStream memoryStream, SmtpReceivedHeader receivedHeader)
        {
            var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Messages");
            var path = Path.Combine(directory, Guid.NewGuid().ToString() + ".eml");

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var stream = new FileStream(path, FileMode.Create))
            {
                var headers = await receivedHeader.GetReceivedHeaders();

                await headers.CopyToAsync(stream);
                await memoryStream.CopyToAsync(stream);
            }
        }
    }

    public class MemoryConfig : IServerConfig
    {
        public string HostName
        {
            get { return Environment.MachineName; }
        }

        public bool IsAuthRequired
        {
            get
            {
                return false;
            }
        }

        public bool IsEncryptionRequired
        {
            get
            {
                return false;
            }
        }

        public bool IsStartTlsSupported
        {
            get
            {
                return false;
            }
        }

        public bool IsTls
        {
            get
            {
                return false;
            }
        }

        public int MaxMessageSize
        {
            get
            {
                return int.MaxValue;
            }
        }

        public int Port
        {
            get
            {
                return 2525;
            }
        }

        public string ServerId
        {
            get
            {
                return "00001";
            }
        }

        public IPAddress ServerIpBinding
        {
            get
            {
                return IPAddress.Any;
            }
        }

        public int SessionTimeout
        {
            get
            {
                return int.MaxValue;
            }
        }

        public X509Certificate2 X509Certificate2
        {
            get
            {
                return null;
            }
        }
    }

    public class TestUserStore : IUserStore
    {
        public string Domain
        {
            get
            {
                return "example.net";
            }
        }

        public void AddUser(IUserIdentity userIdentity)
        {
            throw new NotImplementedException();
        }

        public List<IUserIdentity> GetIdentities()
        {
            throw new NotImplementedException();
        }

        public bool IsUserAuthenticated(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public bool IsValidRecipient(string emailAddress)
        {
            return emailAddress.Contains(this.Domain);
        }
    }
}
