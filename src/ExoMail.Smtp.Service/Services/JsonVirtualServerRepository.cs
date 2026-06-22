using ExoMail.Data;
using ExoMail.Smtp.Common.Interfaces;
using ExoMail.Smtp.Service.Authentication;
using ExoMail.Smtp.Service.Configuration;
using ExoMail.Smtp.Service.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Service.Services
{
    public class JsonVirtualServerRepository : JsonRepository<IVirtualServer>, IVirtualServerRepository
    {
        //private readonly ConfigContext _configContext;
        private readonly VirtualServerFactory _virtualServerFactory;
        private readonly DbContextOptions<ConfigContext> _dbContextOptions;

        public JsonVirtualServerRepository(
            ILoggerFactory loggerFactory,
            IOptions<RepositoryOptions> options,
            DependencyContractResolver contractResolver,
            //ConfigContext configContext,
            DbContextOptions<ConfigContext> dbContextOptions,
            VirtualServerFactory virtualServerFactory
            )
            : base(loggerFactory, options, contractResolver)
        {
            //_configContext = configContext;
            _dbContextOptions = dbContextOptions;
            _virtualServerFactory = virtualServerFactory;
            Initialize();
        }

        internal override string ConfigName => "virtual-servers.json";

        public bool Authenticate(string userName, string password)
        {
            _logger.LogInformation("Attempting to authenticate user: {0}", userName);

            var user = FindByUsername(userName);
            bool authenticated = user != null && user.VerifyPassword(password);

            _logger.LogInformation("User: {0} authenticated: {1}");
            return authenticated;
        }

        public override IVirtualServer Create(IVirtualServer virtualServer)
        {
            _logger.LogDebug("Creating new VirtualServer: {0}", virtualServer.ToString());

            if (String.IsNullOrEmpty(virtualServer.VirtualServerId))
                virtualServer.VirtualServerId = Guid.NewGuid().ToString();

            Items.Add(virtualServer);

            return virtualServer;
        }

        public override void Delete(IVirtualServer item)
        {
            _logger.LogDebug("Deleting VirtualServer: {0}", item.ToString());
            Items.Remove(item);
        }

        public override IVirtualServer Find(string id)
        {
            _logger.LogDebug("Finding VirtualServer with id: {0}", id);
            IVirtualServer virtualServer = Items.FirstOrDefault(x => x.VirtualServerId.Equals(id));

            return virtualServer;
        }

        public IUserIdentity FindByEmailAddress(string emailAddress)
        {
            _logger.LogDebug("Finding UserIdentity with email address: {0}", emailAddress);
            return Items.SelectMany(x => x.Users).FirstOrDefault(x => x.EmailAddresses.Contains(emailAddress, StringComparer.OrdinalIgnoreCase));
        }

        public IUserIdentity FindById(string userId)
        {
            _logger.LogDebug("Finding UserIdentity with userId: {0}", userId);
            return Items.SelectMany(x => x.Users).FirstOrDefault(x => x.UserId.Equals(userId));
        }

        public IUserIdentity FindByUsername(string userName)
        {
            _logger.LogDebug("Finding UserIdentity with username: {0}", userName);
            return Items.SelectMany(x => x.Users).FirstOrDefault(x => x.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        public IVirtualServer FindUserVirtualServer(string userId)
        {
            _logger.LogDebug("Finding VirtualServer for UserIdentity with userId: {0}", userId);

            IUserIdentity user = FindById(userId);
            IVirtualServer server = Find(user.VirtualServerId);

            return server;
        }

        public bool TryAuthenticate(string userName, string password, out IUserIdentity userIdentity)
        {
            _logger.LogDebug("Trying to authenticate username: {0}", userName);
            var user = FindByUsername(userName);

            userIdentity = user;

            return user != null && user.VerifyPassword(password);
        }

        public override void Update(IVirtualServer item)
        {
            _logger.LogDebug("Updating VirtualServer: {0}", item.ToString());
            IVirtualServer virtualServer = Items.First(x => x.VirtualServerId == item.VirtualServerId);

            virtualServer = item;
        }

        public void Initialize()
        {
            //var timer = new Timer((x) => LoadConfigFromDatabase(), null, 0, 10000);

            //LoadConfigFromDatabase();
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        LoadConfigFromDatabase();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                    finally
                    {
                        await Task.Delay(10000);
                    }
                }
            });
        }

        private void LoadConfigFromDatabase()
        {
            try
            {
                using (var configContext = new ConfigContext(_dbContextOptions))
                {
                    var configs = configContext.VirtualServerConfigs
                                   .Include(x => x.Users)
                                   .ThenInclude(x => x.EmailAddresses)
                                   .Include(x => x.Routes)
                                   .Include(x => x.DkimPolicies)
                                   .Select(config => _virtualServerFactory.CreateFromConfig(config));

                    bool updated = false;

                    foreach (var config in configs)
                    {
                        var item = Items.FirstOrDefault(x => x.VirtualServerId == config.VirtualServerId);

                        if (item == null)
                        {
                            _logger.LogInformation("Adding VirtualServer: {0}", config);
                            Items.Add(config);
                            updated = true;
                        }
                    }

                    if (updated)
                        base.SaveChanges();
                }         
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

        }

        public override void ReadConfig()
        {
            base.ReadConfig();
        }
    }
}