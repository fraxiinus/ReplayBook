using System;

namespace Fraxiinus.ReplayBook.Executables.Old.Utilities
{
    public static class StringExtensions
    {
        /// <summary>
        /// Get the substring from beginning to second period '.'
        /// (7.18.201.9160 -> 7.18)
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static string VersionSubstring(this string version)
        {
            try
            {
                if (version == null) { throw new ArgumentNullException(nameof(version)); }
                return version.Substring
                (
                    0,
                    // Get the index of the second period
                    version.IndexOf
                    (
                        '.',
                        // Start after the first period
                        version.IndexOf('.') + 1
                    )
                );
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
