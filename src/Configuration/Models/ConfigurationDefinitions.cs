namespace Fraxiinus.ReplayBook.Configuration.Models;

/// <summary>
/// How should player markers be displayed
/// </summary>
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

/// <summary>
/// Default explorer open option
/// </summary>
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

/// <summary>
/// Application theme style
/// </summary>
public enum Theme
{
    SystemAssigned = 0,
    Light = 1,
    Dark = 2
}

/// <summary>
/// Application display language, ISO 639-1 Language Codes
/// </summary>
public enum ApplicationLanguage
{
    En = 1,
    ZhHans = 2,
    De = 4,
    Es = 8,
    Fr = 16,
    Pt = 32,
    ZhHant = 64,
    Tr = 128
}

/// <summary>
/// Acceptable locales for League of Legends. Used for running League of Legends.exe.
/// When adding a new locale, insert it at the end of the list, but before 'Custom'
/// </summary>
public enum LeagueLocale
{
    Czech, German, Greek, EnglishAU, EnglishGB, EnglishUS, SpanishES, SpanishMX, French,
    Hungarian, Italian, Japanese, Korean, Polish, Portuguese, Romanian, Russian, Turkish,
    ChineseTW, ChineseCN, EnglishPH, EnglishSG, SpanishAR, Indonesian, Thai, Vietnamese,
    ChineseMY, Custom = 99
}

public static class ConfigurationDefinitions
{
    /// <summary>
    /// Dictionary items matches <see cref="ApplicationLanguage"/> order. Number values are used for reverse lookup.
    /// </summary>
    public static readonly Dictionary<string, int> ApplicationLanguageDisplayNames = new()
    {
        { "English", 1 },
        { "简体中文", 2 },
        { "Deutsch", 4 },
        { "Español", 8 },
        { "Français", 16 },
        { "Português (Brasil)", 32 },
        { "繁體中文", 64 },
        { "Türkçe", 128 }
    };

    /// <summary>
    /// Gets corresponding <see cref="ApplicationLanguageDisplayNames"/> index from given <see cref="ApplicationLanguage"/>
    /// </summary>
    /// <param name="language"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static int GetListIndex(this ApplicationLanguage language)
    {
        return language switch
        {
            ApplicationLanguage.En => 0,
            ApplicationLanguage.ZhHans => 1,
            ApplicationLanguage.De => 2,
            ApplicationLanguage.Es => 3,
            ApplicationLanguage.Fr => 4,
            ApplicationLanguage.Pt => 5,
            ApplicationLanguage.ZhHant => 6,
            ApplicationLanguage.Tr => 7,
            _ => throw new NotImplementedException()
        };
    }

    /// <summary>
    /// Gets Riot region code for given <see cref="ApplicationLanguage"/>
    /// </summary>
    /// <param name="language"></param>
    /// <returns></returns>
    public static string GetRiotRegionCode(this ApplicationLanguage language)
    {
        return language switch
        {
            ApplicationLanguage.En => "en_US",
            ApplicationLanguage.ZhHans => "zh_CN",
            ApplicationLanguage.ZhHant => "zh_TW",
            ApplicationLanguage.De => "de_DE",
            ApplicationLanguage.Es => "es_ES",
            ApplicationLanguage.Fr => "fr_FR",
            ApplicationLanguage.Pt => "pt_BR",
            ApplicationLanguage.Tr => "tr_TR",
            _ => "en_US",
        };
    }

    /// <summary>
    /// Gets <see cref="LeagueLocale"/> for given Riot region code
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static LeagueLocale GetLocaleEnum(string name)
    {
        return name switch
        {
            "cs_CZ" => LeagueLocale.Czech,
            "de_DE" => LeagueLocale.German,
            "el_GR" => LeagueLocale.Greek,
            "en_AU" => LeagueLocale.EnglishAU,
            "en_GB" => LeagueLocale.EnglishGB,
            "en_US" => LeagueLocale.EnglishUS,
            "es_ES" => LeagueLocale.SpanishES,
            "es_MX" => LeagueLocale.SpanishMX,
            "fr_FR" => LeagueLocale.French,
            "hu_HU" => LeagueLocale.Hungarian,
            "it_IT" => LeagueLocale.Italian,
            "ja_JP" => LeagueLocale.Japanese,
            "ko_KR" => LeagueLocale.Korean,
            "pl_PL" => LeagueLocale.Polish,
            "pt_BR" => LeagueLocale.Portuguese,
            "ro_RO" => LeagueLocale.Romanian,
            "ru_RU" => LeagueLocale.Russian,
            "tr_TR" => LeagueLocale.Turkish,
            "zh_TW" => LeagueLocale.ChineseTW,
            "zh_CN" => LeagueLocale.ChineseCN,
            "en_PH" => LeagueLocale.EnglishPH,
            "en_SG" => LeagueLocale.EnglishSG,
            "es_AR" => LeagueLocale.SpanishAR,
            "id_ID" => LeagueLocale.Indonesian,
            "th_TH" => LeagueLocale.Thai,
            "vi_VN" => LeagueLocale.Vietnamese,
            "zh_MY" => LeagueLocale.ChineseMY,
            _ => throw new ArgumentException($"locale not found {name}"),
        };
    }

    /// <summary>
    /// Gets Riot region code for given <see cref="LeagueLocale"/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetRiotRegionCode(LeagueLocale name)
    {
        return name switch
        {
            LeagueLocale.Czech => "cs_CZ",
            LeagueLocale.German => "de_DE",
            LeagueLocale.Greek => "el_GR",
            LeagueLocale.EnglishAU => "en_AU",
            LeagueLocale.EnglishGB => "en_GB",
            LeagueLocale.EnglishUS => "en_US",
            LeagueLocale.SpanishES => "es_ES",
            LeagueLocale.SpanishMX => "es_MX",
            LeagueLocale.French => "fr_FR",
            LeagueLocale.Hungarian => "hu_HU",
            LeagueLocale.Italian => "it_IT",
            LeagueLocale.Japanese => "ja_JP",
            LeagueLocale.Korean => "ko_KR",
            LeagueLocale.Polish => "pl_PL",
            LeagueLocale.Portuguese => "pt_BR",
            LeagueLocale.Romanian => "ro_RO",
            LeagueLocale.Russian => "ru_RU",
            LeagueLocale.Turkish => "tr_TR",
            LeagueLocale.ChineseTW => "zh_TW",
            LeagueLocale.ChineseCN => "zh_CN",
            LeagueLocale.EnglishPH => "en_PH",
            LeagueLocale.EnglishSG => "en_SG",
            LeagueLocale.SpanishAR => "es_AR",
            LeagueLocale.Indonesian => "id_ID",
            LeagueLocale.Thai => "th_TH",
            LeagueLocale.Vietnamese => "vi_VN",
            LeagueLocale.ChineseMY => "zh_MY",
            LeagueLocale.Custom => "Custom",
            _ => "en_US",
        };
    }

    /// <summary>
    /// Try to get Riot region code from a string <see cref="LeagueLocale"/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetRiotRegionCode(string name)
    {
        return Enum.TryParse(name, out LeagueLocale result) ? GetRiotRegionCode(result) : GetRiotRegionCode(LeagueLocale.EnglishUS);
    }
}
