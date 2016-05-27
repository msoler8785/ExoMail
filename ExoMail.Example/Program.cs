using ExoMail.Smtp.Authentication;
using ExoMail.Smtp.Interfaces;
using ExoMail.Smtp.Protocol;
using ExoMail.Smtp.Server.Authentication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ExoMail.Example
{
    internal class Program
    {
        private static void Main(string[] args)
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
            var config = MemoryConfig.Create()
                            .WithHostname("exomail01.example.com")
                            .WithPort(2525)
                            .WithServerId("FC30BD4D-1C93-4FBF-BF8F-9788059AF0DC")
                            .WithSessionTimeout( 5 * 60 * 1000 ) // 5 minutes
                            .WithX509Certificate(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "localhost.pfx"), null)
                            .WithEncryptionRequired()
                            .WithStartTlsSupported();

            var messageStore = new FileMessageStore();
            var userStore = new TestUserStore();

            UserManager.GetUserManager.AddUserStore(userStore);

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
                session.SaslMechanisms.Add(new LoginSaslMechanism());
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
            return userName.ToUpper() == "TUSER" && password == "Str0ngP@$$!!";
        }

        public bool IsValidRecipient(string emailAddress)
        {
            return emailAddress.Contains(this.Domain);
        }
    }
}