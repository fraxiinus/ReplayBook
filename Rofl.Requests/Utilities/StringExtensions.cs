using System;
using System.Collections.Generic;
using System.Text;

namespace Rofl.Requests.Utilities
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
