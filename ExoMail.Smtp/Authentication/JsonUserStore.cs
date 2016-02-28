using ExoMail.Smtp.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Authentication
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
        public static JsonUserStore CreateStore()
        {
            if(!File.Exists(_path))
            {
                var store = new JsonUserStore();

                for (int i = 0; i < 5; i++)
                {
                    store.Identities.Add(new JsonUserIdentity() { UserName = "User0" + i, Password = "password" });
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

        public bool IsUserAuthenticated(string userName, string password)
        {
            var storeJson = File.ReadAllText(_path);
            JsonUserStore store = JsonConvert.DeserializeObject<JsonUserStore>(storeJson);
            var user = store.Identities.Find(s => s.UserName.ToUpper() == userName.ToUpper());
            if(user != null)
            {
                return user.Password == password;
            }
            else
            {
                return false;
            }
        }
    }
}
