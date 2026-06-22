using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ExoMail.Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = "Messages";

            var md5 = MD5.Create();

            using (var client = new ImapClient())
            {
                client.Connect("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);
                client.Authenticate("msoler8785@gmail.com", "rinnhamkzgwudvvf");              
                //client.Inbox.Open(FolderAccess.ReadOnly);

                var folders = client
                    .GetFolders(client.PersonalNamespaces.FirstOrDefault())
                    .Where(folder => folder.Exists)
                    .Where(folder => !folder.Attributes.HasFlag(FolderAttributes.All));

                foreach (var folder in folders)
                {
                    Console.WriteLine($"Opening folder: {folder.Name}");
                    folder.Open(FolderAccess.ReadOnly);

                    var currentDir = Path.Combine(dir, folder.Name);
                    var index = Path.Combine(currentDir, "index.txt");
                    Directory.CreateDirectory(currentDir);

                    if (!File.Exists(index))
                        File.WriteAllText(index, String.Empty);

                    var oldUids = File.ReadAllLines(index).Select(x => UniqueId.Parse(x));
                    var uids = folder.Search(SearchQuery.All);
                    var newUids = uids.Except(oldUids);

                    foreach (var uid in newUids)
                    {
                        var message = folder.GetMessage(uid);
                        string file = Path.Combine(currentDir, uid + ".eml");

                        Console.WriteLine($"Downloading message: {message.Subject}");

                        // write the message to a file
                        message.WriteTo(file);

                        File.AppendAllText(index, $"{uid}\r\n");
                    }
                    folder.Close();
                }

                client.Disconnect(true);
            }
        }
    }
}
