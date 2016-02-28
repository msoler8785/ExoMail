using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Enums
{
    public enum SaslType
    {
        ANONYMOUS,
        PLAIN,
        LOGIN,
        DIGEST_MD5,
        CRAM_MD5,
        GSSAPI
    }
}
