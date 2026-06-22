using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Service.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Service.Data
{
    public abstract class JsonRepository<T> : IRepository<T>
    {
        internal abstract string ConfigName { get; }
        //internal virtual Action ConfigReadAction => () => { };
        private readonly string _configPath;
        protected readonly ILogger _logger;
        private readonly JsonSerializer _serializer;
        private readonly RepositoryOptions _options;
        private readonly FileSystemWatcher _fileWatcher;
        private readonly object _lock = new object();
        //private byte[] _fileHash;
        private MD5 _md5;
        private DateTime _modifiedTime;
        private DateTime _updateTime;     
        //public DateTime ModifiedTime { get => _modifiedTime; set => _modifiedTime = value; }

        public JsonRepository(
            ILoggerFactory loggerFactory, 
            IOptions<RepositoryOptions> options, 
            DependencyContractResolver contractResolver
            )
        {
            _options = options.Value;
            //ConfigName = typeof(T).Name + ".json";
            _configPath = Path.Combine(_options.ConfigBase, ConfigName);
            _logger = loggerFactory.CreateLogger<JsonRepository<T>>();
            _serializer = new JsonSerializer()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            };
            Items = new List<T>();
            _md5 = MD5.Create();
            _updateTime = DateTime.Now;
            _fileWatcher = new FileSystemWatcher(_options.ConfigBase) { NotifyFilter = NotifyFilters.LastWrite };
            _fileWatcher.Changed += OnConfigUpdated;
            _fileWatcher.Filter = ConfigName;
            _fileWatcher.EnableRaisingEvents = true;
        }

        public List<T> Items { get; set; }

        public virtual T Create(T item)
        {
            _logger.LogDebug("Creating new {0}", nameof(T));

            Items.Add(item);

            return item;
        }

        public virtual void Delete(T item)
        {
            throw new NotImplementedException();
        }

        public virtual T Find(string id)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(T item)
        {
            throw new NotImplementedException();
        }

        public virtual void ReadConfig()
        {
            lock (_lock)
            {
                bool updated = false;

                if (!File.Exists(_configPath))
                {
                    Directory.CreateDirectory(_options.ConfigBase);
                }
                else
                {
                    while (!updated)
                    {
                        FileStream fs = null;

                        try
                        {
                            fs = new FileStream(_configPath, FileMode.Open, FileAccess.Read);

                            using (StreamReader sr = new StreamReader(fs))
                            using (JsonReader jr = new JsonTextReader(sr))
                            {
                                _logger.LogInformation("Reading repository from {0}", _configPath);
                                Items = _serializer.Deserialize<List<T>>(jr);
                            }

                            updated = true;
                        }
                        catch (IOException)
                        {
                            _logger.LogDebug("File is locked for reading.");
                            Thread.Sleep(1000);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.HResult, ex, ex.Message);
                        }
                        finally
                        {
                            fs?.Dispose();
                        }
                    }

                    OnConfigReloaded();
                }
            }
        }

        internal virtual void OnConfigReloaded()
        {
        }

        private void OnConfigUpdated(object sender, FileSystemEventArgs e)
        {
            lock (_lock)
            {
                _modifiedTime = DateTime.Now;
                const int changeThreshold = 2;

                if (_modifiedTime >= _updateTime)
                {
                    _logger.LogInformation("Change detected in configuration file: {0}", e.FullPath);
                    ReadConfig();
                    _updateTime = _modifiedTime.AddSeconds(changeThreshold);
                }
            }
        }

        public void SaveChanges()
        {
            lock (_lock)
            {
                _logger.LogInformation("Saving changes to repository");
                _modifiedTime = DateTime.Now;
                bool updated = false;
                while (!updated)
                {
                    FileStream fs = null;

                    try
                    {
                        fs = new FileStream(_configPath, FileMode.Create, FileAccess.Write);
                        using (StreamWriter sr = new StreamWriter(fs))
                        using (JsonWriter jw = new JsonTextWriter(sr))
                        {
                            _serializer.Serialize(jw, Items);
                        }
                        updated = true;
                        _logger.LogInformation("Changes saved to repository");
                    }
                    catch (IOException)
                    {
                        _logger.LogDebug("File is locked for writing.");
                        Thread.Sleep(1000);
                    }
                    finally
                    {
                        fs?.Dispose();
                    }
                }
            }
        }
    }
}