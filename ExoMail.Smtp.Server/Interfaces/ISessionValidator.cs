using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Interfaces
{
    public interface ISessionValidator
    {
        bool IsValid();
    }
}
