using System.Text;

namespace ExoMail.Smtp.Common.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendLineFormat(this StringBuilder stringBuilder, string format, params object[] args)
        {
            stringBuilder.AppendFormat(format, args);
            stringBuilder.Append("\r\n");
            return stringBuilder;
        }
    }
}