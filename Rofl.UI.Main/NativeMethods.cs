using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.UI.Main
{
    internal static class NativeMethods
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        internal static extern long StrFormatByteSizeW(long qdw, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszBuf, int cchBuf);
    }
}
