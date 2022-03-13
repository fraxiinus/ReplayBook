using System.Reflection;

namespace Fraxiinus.ReplayBook.UI.Main.Utilities
{
    /// <summary>
    /// Contains helper properties for identifying the application.
    /// </summary>
    public static class ApplicationProperties
    {
        public static string Version { get => Assembly.GetExecutingAssembly().GetName().Version.ToString(3); }

        public static string UserAgent { get => $"ReplayBook/{Version} (+https://github.com/fraxiinus/ReplayBook)"; }
    }
}
