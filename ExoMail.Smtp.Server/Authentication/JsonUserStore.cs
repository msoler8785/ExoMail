using ExoMail.Smtp.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ExoMail.Smtp.Server.Authentication
{
    /// <summary>
    /// Simple implementation of a UserStore using JSON.
    /// Should be considered insecure.
    /// </summary>
    public class JsonUserStore : IUserStore
    {
        private static string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users.json");

        public string Domain { get; set; }

        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.Auto)]
        public List<IUserIdentity> Identities { get; set; }

        public JsonUserStore()
        {
            this.Identities = new List<IUserIdentity>();
        }

        //Creates a sample JsonUserStore
        public static JsonUserStore CreateStore(string domain)
        {
            if (!File.Exists(_path))
            {
                var store = new JsonUserStore() { Domain = domain };

                for (int i = 1; i < 1000; i++)
                {
                    var identity = new JsonUserIdentity()
                    {
                        UserName = "User0" + i,
                        EmailAddress = String.Format("User0{0}@{1}", i, domain),
                        AliasAddresses = new List<string>() { String.Format("Alias0{0}@{1}", i, domain) },
                        FirstName = "Test",
                        LastName = "User0" + i,
                        MailboxPath = Path.Combine(domain, "User0" + i)
                    };
                    identity.Password = HashPassword(identity.UserId + "password");
                    store.Identities.Add(identity);
                }
                var storeJson = JsonConvert.SerializeObject(store, Formatting.Indented);
                File.WriteAllText(_path, storeJson);
                return store;
            }
            else
            {
                return new JsonUserStore();
            }
        }

        public List<IUserIdentity> GetIdentities()
        {
            if (File.Exists(_path))
            {
                var storeJson = File.ReadAllText(_path);
                JsonUserStore store = JsonConvert.DeserializeObject<JsonUserStore>(storeJson);
                return store.Identities;
            }
            else
            {
                return new List<IUserIdentity>();
            }
        }

        public bool IsUserAuthenticated(string userName, string password)
        {
            //var storeJson = File.ReadAllText(_path);
            //JsonUserStore store = JsonConvert.DeserializeObject<JsonUserStore>(storeJson);
            var user = GetIdentities().Find(s => s.UserName.ToUpper() == userName.ToUpper());
            if (user != null)
            {
                return user.Password == HashPassword(user.UserId + password);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the forward path contains a valid recipient email address.
        /// </summary>
        /// <param name="forwardPath">
        /// A forward-path (normally a mailbox and domain, always surrounded by "&lt;" and "&gt;" brackets)
        /// identifying one recipient.
        /// Ex. &lt;jdoe@example.net&gt;
        /// <see cref="https://tools.ietf.org/html/rfc5321#section-4.1.2"/>
        /// </param>
        /// <returns>True if the recipient address is in the UserStore and is availiable for delivery.</returns>
        public bool IsValidRecipient(string forwardPath)
        {
            forwardPath = Regex.Match(forwardPath, @"<(.*)>").Groups[1].Value;
            //var storeJson = File.ReadAllText(_path);
            //JsonUserStore store = JsonConvert.DeserializeObject<JsonUserStore>(storeJson);
            bool validEmail = GetIdentities().Any(x => x.EmailAddress.ToUpper() == forwardPath.ToUpper());
            bool validAlias = GetIdentities().Any(x => x.AliasAddresses.Any(a => a.ToUpper().Contains(forwardPath.ToUpper())));
            return validEmail || validAlias;
        }

        private static string HashPassword(string password)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var hash = SHA256.Create().ComputeHash(passwordBytes);
            password = Convert.ToBase64String(hash);
            return password;
        }

        public void AddUser(IUserIdentity userIdentity)
        {
            throw new NotImplementedException();

            var user = new JsonUserIdentity()
            {
                FirstName = userIdentity.FirstName,
                LastName = userIdentity.LastName,
                EmailAddress = userIdentity.EmailAddress,
                AliasAddresses = userIdentity.AliasAddresses,
                MailboxPath = Path.Combine(domain, userIdentity.UserName),
                UserName = userIdentity.UserName                
            };
            user.Password = HashPassword(user.UserId + userIdentity.Password);

            var identities = GetIdentities();

            // TODO: Verify user doesn't already exists in store.
            identities.Add(user);

            string json = JsonConvert.SerializeObject(identities, Formatting.Indented);
            File.WriteAllText(_path, json);
        }
    }
}