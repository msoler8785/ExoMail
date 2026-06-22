using System;

namespace ExoMail.Smtp.Common.Enums
{
    [Flags]
    public enum Disposition
    {
        Queued = 1,
        Delivered = 2,
        TempFail = 4,
        PermFail = 8,
        Deferred = 16,
        Retryable = 21
    }

    public static class DispositionExtensions
    {
        public static bool HasDisposition(this Disposition @this, Disposition disposition)
        {
            return (@this & disposition) != 0;
        }
    }
}