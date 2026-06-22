using ExoMail.Smtp.Common.Services;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExoMail.Smtp.Service
{
    public class DependencyContractResolver : DefaultContractResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IResolver _resolver;

        public DependencyContractResolver(IServiceProvider serviceProvider, IResolver resolver)
        {
            _serviceProvider = serviceProvider;
            _resolver = resolver;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            //var service = _serviceProvider.GetService(objectType);
            bool resolved = _resolver.Exists(objectType);

            if (resolved)
            {
                JsonObjectContract contract = base.CreateObjectContract(objectType);
                contract.DefaultCreator = () => _serviceProvider.GetService(objectType);
                return contract;
            }

            return base.CreateObjectContract(objectType);
        }
    }
}