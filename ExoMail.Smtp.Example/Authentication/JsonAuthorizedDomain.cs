using ExoMail.Smtp.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Authentication
{
    public class JsonAuthorizedDomain : IAuthorizedDomain
    {
        private static string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Domains.txt");
        public string DomainName { get; set; }

        [JsonProperty(TypeNameHandling = TypeNameHandling.Auto)]
        public IUserStore UserStore { get; set; }

        public JsonAuthorizedDomain()
        {
        }

        public static List<JsonAuthorizedDomain> Create()
        {
            if (!File.Exists(_path))
            {
                string[] domains = { "example.com", "example.org" };
                var configs = new List<JsonAuthorizedDomain>();

                foreach (var item in domains)
                {
                    var config = new JsonAuthorizedDomain()
                    {
                        DomainName = item,
                        UserStore = JsonUserStore.CreateStore(item)
                    };
                    configs.Add(config);
                }

                var json = JsonConvert.SerializeObject(configs, Formatting.Indented);
                File.WriteAllText(_path, json);
                return configs;
            }
            else
            {
                var json = File.ReadAllText(_path);
                var configs = JsonConvert.DeserializeObject<List<JsonAuthorizedDomain>>(json);
               
                return configs ;
            }
        }
    }
}
