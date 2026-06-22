using System;
using System.Collections.Generic;
using System.Text;

namespace ExoMail.Smtp.Common.Interfaces
{
    public interface IRepository<TItem>
    {
        TItem Create(TItem item);
        TItem Find(string id);
        void Update(TItem item);
        void Delete(TItem item);
        void SaveChanges();
    }
}
