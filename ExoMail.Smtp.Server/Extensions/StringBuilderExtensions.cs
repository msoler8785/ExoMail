using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExoMail.Smtp.Server.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendLineFormat(this StringBuilder stringBuilder, string format, params object[] args )
        {
            stringBuilder.AppendFormat(format + "\r\n", args);
            return stringBuilder;
        }
    }
}
