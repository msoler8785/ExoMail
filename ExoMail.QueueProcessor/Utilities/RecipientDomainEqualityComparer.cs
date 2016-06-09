using ExoMail.Smtp.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.QueueProcessor.Utilities
{
    public class RecipientDomainEqualityComparer : IEqualityComparer<Recipient>
    {
        public bool Equals(Recipient x, Recipient y)
        {
            return x.RecipientDomain.ToUpper() == y.RecipientDomain.ToUpper();
        }

        public int GetHashCode(Recipient obj)
        {
            throw new NotImplementedException();
        }
    }
}
