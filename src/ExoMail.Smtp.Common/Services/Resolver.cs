using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace ExoMail.Smtp.Common.Services
{
    public class Resolver : IResolver
    {
        private readonly IServiceCollection _serviceCollection;

        public Resolver(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public bool Exists(Type type)
        {
            return _serviceCollection.Any(x => x.ServiceType.Equals(type));
        }
    }
}
