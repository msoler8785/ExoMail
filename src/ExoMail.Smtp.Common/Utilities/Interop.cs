using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ExoMail.Smtp.Common.Utilities
{
    public class Interop
    {
        public static bool IsLinux()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }

        public static bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }
    }
}