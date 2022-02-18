/// Based on example by Kirill Osenkov 
/// https://stackoverflow.com/a/44816953
using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace Fraxiinus.ReplayBook.UI.Main.Utilities
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
            string filePath = Environment.ProcessPath;

            ClearExplorerKeys();

            Set(new FileAssociation
            {
                Extension = ".rofl",
                ProgId = "ReplayBook",
                FileTypeDescription = "League of Legends Replay File",
                ExecutableFilePath = filePath
            });
        }

        /// <summary>
        /// File associations set by the user in explorer are stored in a different place
        /// </summary>
        private static void ClearExplorerKeys()
        {
            // Deleting keys one at a time as DeleteSubKeyTree was causing errors
            Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.rofl\UserChoice", false);
            Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.rofl\OpenWithProgids", false);
            Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.rofl\OpenWithList", false);
            Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.rofl", false);

            Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\Classes\.rofl", false);
            Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\Classes\rofl_auto_file\shell\open\command", false);
            Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\Classes\rofl_auto_file\shell\open", false);
            Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\Classes\rofl_auto_file\shell", false);
            Registry.CurrentUser.DeleteSubKey(@"SOFTWARE\Classes\rofl_auto_file", false);
        }

        private static void Set(params FileAssociation[] associations)
        {
            // This flat is used to determine if changes were made in the registry
            bool madeChanges = false;
            // Apply our associations
            foreach (FileAssociation association in associations)
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
            bool madeChanges = false;
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + extension, progId);
            madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + progId, fileTypeDescription);
            madeChanges |= SetKeyDefaultValue($@"Software\Classes\{progId}\shell\open\command", "\"" + applicationFilePath + "\" \"%1\"");
            return madeChanges;
        }

        private static bool SetKeyDefaultValue(string keyPath, string value)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(keyPath))
            {
                if ((key.GetValue(null) as string) != value)
                {
                    key.SetValue(null, value);
                    return true;
                }
            }

            return false;
        }
    }
}
