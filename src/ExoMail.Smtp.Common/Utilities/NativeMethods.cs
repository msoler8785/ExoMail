using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ExoMail.Smtp.Common.Utilities
{
    public class NativeMethods
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcmp(byte[] b1, byte[] b2, long count);

        public static bool MemCmp(byte[] b1, byte[] b2)
        {
            // Validate buffers are the same length.
            // This also ensures that the count does not exceed the length of either buffer.
            return b1.Length == b2.Length && memcmp(b1, b2, b1.Length) == 0;
        }
    }
}