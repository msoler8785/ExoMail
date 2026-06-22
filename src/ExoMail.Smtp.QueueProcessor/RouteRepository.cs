using ExoMail.Smtp.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExoMail.Smtp.QueueProcessor
{
    public class RouteRepository : IRouteRepository
    {
        private object _lock = new object();
        private readonly HashSet<IMessageRoute> _routes;

        private const string BASE_PATH = "Configs";
        private const string CONF_PATH = BASE_PATH + "\\routes.json";

        private static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };

        private readonly ILoggerFactory _loggerFactory;

        public HashSet<IMessageRoute> Routes => _routes;

        public RouteRepository(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            if (File.Exists(CONF_PATH))
            {
                string json = File.ReadAllText(CONF_PATH);
                _routes = JsonConvert.DeserializeObject<HashSet<IMessageRoute>>(json);
            }
            else
            {
                _routes = new HashSet<IMessageRoute>();
            }
        }

        public IMessageRoute Create(IMessageRoute item)
        {
            _routes.Add(item);

            return item;
        }

        public void Delete(IMessageRoute item)
        {
            _routes.Remove(item);
        }

        public IMessageRoute Find(string id)
        {
            IMessageRoute route = _routes.FirstOrDefault(x => x.MessageRouteId == id);
            return route;
        }

        public void Update(IMessageRoute item)
        {
            IMessageRoute route = _routes.FirstOrDefault(x => x.Equals(item));

            if (route == null)
            {
                route = _routes.FirstOrDefault(x => x.MessageRouteId == item.MessageRouteId);
                route = item;
            }

            if (route == null)
            {
                throw new ArgumentException($"Unable to update route with id {item.MessageRouteId}. Route doesn't exist in collection.");
            }
        }

        public void PersistChanges()
        {
            lock (_lock)
            {
                if (!Directory.Exists(BASE_PATH))
                    Directory.CreateDirectory(BASE_PATH);

                var json = JsonConvert.SerializeObject(_routes, _jsonSettings);

                File.WriteAllText(CONF_PATH, json);
            }
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}