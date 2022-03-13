using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Fraxiinus.ReplayBook.UI.Main
{
    internal static class NativeMethods
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        internal static extern long StrFormatByteSizeW(long qdw, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszBuf, int cchBuf);

        [DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        // Used by file association code to notify explorer that association has changed
        [DllImport("shell32.dll")]
        internal static extern void SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);
    }
}
