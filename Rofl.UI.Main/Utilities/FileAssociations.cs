/// Based on example by Kirill Osenkov 
/// https://stackoverflow.com/a/44816953
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.UI.Main.Utilities
{
    public class FileAssociation
    {
        public string Extension { get; set; }
        public string ProgId { get; set; }
        public string FileTypeDescription { get; set; }
        public string ExecutableFilePath { get; set; }
    }

    public static class FileAssociations
    {
        private const int SHCNE_ASSOCCHANGED = 0x8000000;
        private const int SHCNF_FLUSH = 0x1000;

        public static void SetRoflToSelf()
        {
            var filePath = Process.GetCurrentProcess().MainModule.FileName;

            FileAssociations.Set(new FileAssociation
            {
                Extension = ".rofl",
                ProgId = "LOL_Replay_File",
                FileTypeDescription = "League of Legends Replay File",
                ExecutableFilePath = filePath
            });
        }

        public static void Set(params FileAssociation[] associations)
        {
            // This flat is used to determine if changes were made in the registry
            bool madeChanges = false;
            // Apply our associations
            foreach (var association in associations)
            {
                madeChanges |= SaveToRegistry(association.Extension,
                    association.ProgId,
                    association.FileTypeDescription,
                    association.ExecutableFilePath);
            }

            // If changes were made, we need to tell explorer to refresh
            if (madeChanges)
            {
                NativeMethods.SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_FLUSH, IntPtr.Zero, IntPtr.Zero);
            }
        }

        private static bool SaveToRegistry(string extension, string progId, string fileTypeDescription, string applicationFilePath)
        {
            return SetRegistryKey(@"Software\Classes\" + extension, progId) ||
                   SetRegistryKey(@"Software\Classes\" + progId, fileTypeDescription) ||
                   SetRegistryKey($@"Software\Classes\{progId}\shell\open\command", "\"" + applicationFilePath + "\" \"%1\"");
        }

        private static bool SetRegistryKey(string keyPath, string value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(keyPath))
            {
                if (key.GetValue(null) as string != value)
                {
                    key.SetValue(null, value);
                    return true;
                }
            }

            return false;
        }
    }
}
