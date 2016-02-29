using ExoMail.Smtp.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace ExoMail.Smtp.Server.Utilities
{
    public class JsonConfig : IServerConfig
    {
        [JsonIgnore]
        private static string _path = AppDomain.CurrentDomain.BaseDirectory;

        public string IpBindingString { get; set; }
        public string ServerId { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; }
        public int SessionTimeout { get; set; }
        public int MaxMessageSize { get; set; }

        [JsonIgnore]
        public List<ISessionValidator> SessionValidators { get; set; }

        [JsonIgnore]
        public List<IMessageValidator> MessageValidators { get; set; }

        [JsonIgnore]
        public X509Certificate2 X509Certificate2 { get; set; }

        [JsonIgnore]
        public IPAddress ServerIpBinding
        {
            get { return IPAddress.Parse(this.IpBindingString); }
            set { this.IpBindingString = value.ToString(); }
        }

        public bool IsEncryptionRequired { get; set; }
        public bool IsTls { get; set; }

        private JsonConfig()
        {
        }

        public static void CreateDefaultConfig()
        {
            List<JsonConfig> configs = new List<JsonConfig>();
            string hostName = Environment.MachineName;
            int[] ports = { 2525, 465, 587 };

            foreach (var port in ports)
            {
                bool isEncryptionRequired = port == 587;
                bool isTls = port == 465;
                configs.Add(new JsonConfig()
                {
                    ServerId = Guid.NewGuid().ToString(),
                    HostName = hostName,
                    Port = port,
                    IpBindingString = "0.0.0.0",
                    IsEncryptionRequired = isEncryptionRequired,
                    IsTls = isTls,
                    MaxMessageSize = 25 * 1024 * 1024,
                    SessionTimeout = 10 * 60 * 1000,
                });
            }

            string configFile = JsonConvert.SerializeObject(configs, Formatting.Indented);
            string configPath = Path.Combine(_path, "ServerConfig.txt");
            File.WriteAllText(configPath, configFile);
        }

        /// <summary>
        /// Loads the config file from disk.
        /// </summary>
        public static List<JsonConfig> LoadConfigs()
        {
            string configPath = Path.Combine(_path, "ServerConfig.txt");
            if (!File.Exists(configPath))
            {
                CreateDefaultConfig();
            }
            string config = File.ReadAllText(configPath);
            List<JsonConfig> configs = JsonConvert.DeserializeObject<List<JsonConfig>>(config);

            return configs;
        }
    }
}