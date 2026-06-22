using ExoMail.Smtp.Client.Network;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace ExoMail.UtilityTest
{
    class Program
    {
        static void Main(string[] args)
        {

            //var host = Dns.GetHostAddresses("ho-pr-rec01.corp.socketlabs.com");
            var host = Dns.GetHostAddresses("localhost");

            SmtpClient smtp = null; 

            try
            {
                smtp = new SmtpClient(host[1], userName: "Socket", password: "SenderScore100");
                //smtp = new SmtpClient(host[0]);
                smtp.ConnectAsync().Wait();

                if (smtp.IsConnected)
                {
                    //smtp.ServerExtensions &= ~Smtp.Client.Enums.ServerExtensions.SIZE;
                    //smtp.SendAsync("matt.soler@socketlabs.com", new string[] { "matt.soler@socketlabs.com" }, "huge-message.eml").Wait();
                }
            }
            catch (AggregateException ex)
            {
                foreach (var exception in ex.InnerExceptions)
                {
                    Console.WriteLine(exception.Message);
                }
            }
            finally
            {
                smtp?.Dispose();
            }
        }
    }
}
