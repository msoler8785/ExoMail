using ExoMail.Smtp.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;

namespace ExoMail.Smtp.Common.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddResolver(this IServiceCollection serviceCollection)
        {
            Resolver resolver = new Resolver(serviceCollection);
            serviceCollection.AddSingleton<IResolver, Resolver>((x) => resolver);
            return serviceCollection;
        }

        public static IServiceCollection ImportPlugins<TDefinition>(this IServiceCollection serviceCollection)
        {
            var path = Directory.CreateDirectory("Plugins");
            var binaries = path.EnumerateFiles("*.dll", SearchOption.AllDirectories);

            binaries.SelectMany(module =>
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(module.FullName);
                var types = assembly.ExportedTypes;
                return types;
            })
            .Where(type => type.GetInterfaces().Contains(typeof(TDefinition)))
                .ToList()
                .ForEach(type => serviceCollection.AddSingleton(typeof(TDefinition), Activator.CreateInstance(type)));

            return serviceCollection;
        }

        public static T GetRequiredService<T>(this IServiceProvider serviceProvider, Type type)
        {
            var service = serviceProvider.GetRequiredService(type);
            


            return (T)service;
        }

        //public static IServiceCollection LoadSmtpServers(this IServiceCollection serviceCollection)
        //{


        //    var servers = serviceCollection.PostConfigure<IEnumerable<SmtpServerOptions>>((x) => 
        //    {
        //        foreach (var item in x)
        //        {

        //        }
        //    });
                
        //}
    }
}