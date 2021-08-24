using System.Reflection;

namespace Rofl.UI.Main.Utilities
{
    /// <summary>
    /// Contains helper functions for identifying the application.
    /// </summary>
    public static class ApplicationHelper
    {
        public static string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        }

        public static string GetUserAgent()
        {
            return $"ReplayBook/{GetVersion()} (+https://github.com/fraxiinus/ReplayBook)";
        }
    }
}
