using ExoMail.Smtp.Server.Enums;

namespace ExoMail.Smtp.Server.Extensions
{
    public static class EnumExtensions
    {
        public static bool ContainsFlag(this SessionState flags, SessionState flag)
        {
            return (flags & flag) != 0;
        }
    }
}