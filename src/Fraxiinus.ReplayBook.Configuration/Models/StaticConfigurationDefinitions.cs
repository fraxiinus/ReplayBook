namespace Fraxiinus.ReplayBook.Configuration.Models
{
    public enum MarkerStyle
    {
        /// <summary>
        /// Show player marker as a colored border around the icon
        /// </summary>
        Border = 0,

        /// <summary>
        /// Show player marker as a square in the top right of the icon
        /// </summary>
        Square = 1
    }

    public enum FileAction
    {
        /// <summary>
        /// Play replays when opened in Explorer
        /// </summary>
        Play = 0,

        /// <summary>
        /// Open replays in ReplayBook when opened in Explorer
        /// </summary>
        Open = 1
    }

    public enum Theme
    {
        SystemAssigned = 0,
        Light = 1,
        Dark = 2
    }

    public enum Language
    {
        En, ZhHans, De, Es, Fr, Pt, ZhHant
    }

    public class StaticConfigurationDefinitions
    {
        /// <summary>
        /// Dictionary items matches <see cref="Language"/> order. Number values are used for reverse lookup.
        /// </summary>
        public static readonly Dictionary<string, int> LanguageDisplayNames = new()
        {
            { "English", 0 },
            { "简体中文", 1 },
            { "Deutsch", 2 },
            { "Español", 3 },
            { "Français", 4 },
            { "Português (Brasil)", 5 },
            { "繁體中文", 6 }
        };
    }
}
