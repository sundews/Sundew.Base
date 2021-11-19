using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace Sundew.Test.Infrastructure.Text;

public class Win32NaturalTextComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == null)
        {
            return y == null ? 0 : -1;
        }

        return y == null ? 1 : SafeNativeMethods.StrCmpLogicalW(x, y);
    }

    [SuppressUnmanagedCodeSecurity]
    internal static class SafeNativeMethods
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int StrCmpLogicalW(string psz1, string psz2);
    }
}
