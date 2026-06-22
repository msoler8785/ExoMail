using ExoMail.Smtp.Client.Enums;

namespace ExoMail.Smtp.Client.Extensions
{
    internal static class EnumExtensions
    {
        public static bool HasExtension(this ServerExtensions enumeration, ServerExtensions serverExtensions)
        {
            return (enumeration & serverExtensions) != 0;
        }
    }
}