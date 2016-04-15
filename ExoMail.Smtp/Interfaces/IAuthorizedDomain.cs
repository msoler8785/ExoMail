﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Interfaces
{
    public interface IAuthorizedDomain
    {
        string DomainName { get; set; }
        IUserStore UserStore { get; set; }
    }
}
