namespace ExoMail.Smtp.Utilities
{
    /// <summary>
    /// Helps to get number of bytes in byte unit prefix.
    /// </summary>
    public static class ByteSizeHelper
    {
        public const int KILOBYTE = 1024;
        public const int MEGABYTE = KILOBYTE * 1024;
        public const int GIGABYTE = MEGABYTE * 1024;

        /// <summary>
        /// Get the number of bytes in the specified prefix.
        /// </summary>
        /// <param name="kilobytes">The number of Kilobytes to conver.t</param>
        /// <returns>An integer for the number of bytes.</returns>
        public static int FromKiloBytes(int kilobytes)
        {
            return KILOBYTE * kilobytes;
        }

        /// <summary>
        /// Get the number of bytes in the specified prefix.
        /// </summary>
        /// <param name="megabytes">The number of megabytes to convert.</param>
        /// <returns>An integer for the number of bytes.</returns>
        public static int FromMegaBytes(int megabytes)
        {
            return MEGABYTE * megabytes;
        }

        /// <summary>
        /// Get the number of bytes in the specified prefix.
        /// </summary>
        /// <param name="gigabytes">The number of gigabytes to convert.</param>
        /// <returns>An integer for the number of bytes.</returns>
        public static int FromGigaBytes(int gigabytes)
        {
            return GIGABYTE * gigabytes;
        }
    }
}
