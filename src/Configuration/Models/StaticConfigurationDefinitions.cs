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

    public enum ProgramLanguage
    {
        En = 1,
        ZhHans = 2,
        De = 4,
        Es = 8,
        Fr = 16,
        Pt = 32,
        ZhHant = 64
    }

    public class StaticConfigurationDefinitions
    {
        /// <summary>
        /// Dictionary items matches <see cref="ProgramLanguage"/> order. Number values are used for reverse lookup.
        /// </summary>
        public static readonly Dictionary<string, int> LanguageDisplayNames = new()
        {
            { "English", 1 },
            { "简体中文", 2 },
            { "Deutsch", 4 },
            { "Español", 8 },
            { "Français", 16 },
            { "Português (Brasil)", 32 },
            { "繁體中文", 64 }
        };
    }
}
