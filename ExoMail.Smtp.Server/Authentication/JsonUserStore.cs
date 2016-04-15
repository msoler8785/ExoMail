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
        private static string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Users.txt");

        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.Auto)]
        public List<IUserIdentity> Identities { get; set; }

        public string Realm { get; set; }

        public JsonUserStore()
        {
            this.Identities = new List<IUserIdentity>();
            this.Realm = String.Empty;
        }

        //Creates a sample JsonUserStore
        public static JsonUserStore CreateStore(string domain)
        {
            if (!File.Exists(_path))
            {
                var store = new JsonUserStore();

                for (int i = 0; i < 5; i++)
                {
                    store.Identities.Add(new JsonUserIdentity()
                    {
                        UserName = "User0" + i,
                        Password = HashPassword("password"),
                        EmailAddress = String.Format("User0{0}@{1}", i, domain)
                    });
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

        private static string HashPassword(string password)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var hash = SHA256.Create().ComputeHash(passwordBytes);
            password = Convert.ToBase64String(hash);
            return password;
        }

        public bool IsUserAuthenticated(string userName, string password)
        {
            var storeJson = File.ReadAllText(_path);
            JsonUserStore store = JsonConvert.DeserializeObject<JsonUserStore>(storeJson);
            var user = store.Identities.Find(s => s.UserName.ToUpper() == userName.ToUpper());
            if (user != null)
            {
                return user.Password == HashPassword(password);
            }
            else
            {
                return false;
            }
        }

        public bool IsValidRecipient(string emailAddress)
        {
            emailAddress = Regex.Match(emailAddress, @"<(.*)>").Groups[0].Value;
            var storeJson = File.ReadAllText(_path);
            JsonUserStore store = JsonConvert.DeserializeObject<JsonUserStore>(storeJson);

            return store.Identities.Any(x => x.EmailAddress.ToUpper() == emailAddress.ToUpper());
        }
    }
}