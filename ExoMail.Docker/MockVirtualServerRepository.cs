using ExoMail.Smtp.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Docker
{
    public class MockVirtualServerRepository : IVirtualServerRepository
    {
        public List<IVirtualServer> Items => throw new NotImplementedException();

        public IVirtualServer Create(IVirtualServer item)
        {
            throw new NotImplementedException();
        }

        public void Delete(IVirtualServer item)
        {
            throw new NotImplementedException();
        }

        public IVirtualServer Find(string id)
        {
            throw new NotImplementedException();
        }

        public IUserIdentity FindByEmailAddress(string emailAddress)
        {
            var userIdentity = new MockUserIdentity();
            return userIdentity;
        }

        public IUserIdentity FindById(string userId)
        {
            throw new NotImplementedException();
        }

        public IUserIdentity FindByUsername(string userName)
        {
            throw new NotImplementedException();
        }

        public IVirtualServer FindUserVirtualServer(string userId)
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public bool TryAuthenticate(string userName, string password, out IUserIdentity userIdentity)
        {
            userIdentity = new MockUserIdentity();

            return true;
        }

        public void Update(IVirtualServer item)
        {
            throw new NotImplementedException();
        }
    }
}
