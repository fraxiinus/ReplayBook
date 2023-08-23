using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;

namespace Fraxiinus.ReplayBook.Configuration.Models
{
    public class ReplaySettings
    {
        [ConfigurationKeyName("folder_list")]
        [JsonPropertyName("folder_list")]
        public string[]? FolderList { get; set; }

        [ConfigurationKeyName("search_minimum_score")]
        [JsonPropertyName("search_minimum_score")]
        public float SearchMinimumScore { get; set; } = 0.3f;
    }

    public class RequestSettings
    {
        [ConfigurationKeyName("ddragon_baseurl")]
        [JsonPropertyName("ddragon_baseurl")]
        public string DataDragonBaseUrl { get; set; } = "https://ddragon.leagueoflegends.com/cdn/";


        [ConfigurationKeyName("cdragon_baseurl")]
        [JsonPropertyName("cdragon_baseurl")]
        public string CommunityDragonBaseUrl { get; set; } = "https://raw.communitydragon.org/";

        [ConfigurationKeyName("staticdata_useapplanguage")]
        [JsonPropertyName("staticdata_useapplanguage")]
        public bool UseCurrentLanguageAsLocale { get; set; } = true;

        [ConfigurationKeyName("staticdata_downloadlanguage")]
        [JsonPropertyName("staticdata_downloadlanguage")]
        public LeagueLocale StaticDataDownloadLanguage { get; set; } = LeagueLocale.EnglishUS;
    }

    public class GeneralSettings
    {
        [ConfigurationKeyName("known_players")]
        [JsonPropertyName("known_players")]
        public PlayerMarkerConfiguration[]? KnownPlayers { get; set; }

        [ConfigurationKeyName("marker_style")]
        [JsonPropertyName("marker_style")]
        public MarkerStyle MarkerStyle { get; set; }

        [ConfigurationKeyName("file_action")]
        [JsonPropertyName("file_action")]
        public FileAction FileAction { get; set; }

        [ConfigurationKeyName("play_confirm")]
        [JsonPropertyName("play_confirm")]
        public bool PlayConfirm { get; set; }

        [ConfigurationKeyName("rename_file")]
        [JsonPropertyName("rename_file")]
        public bool RenameFile { get; set; }

        [ConfigurationKeyName("items_per_page")]
        [JsonPropertyName("items_per_page")]
        public int ItemsPerPage { get; set; }

        [ConfigurationKeyName("auto_update_check")]
        [JsonPropertyName("auto_update_check")]
        public bool AutoUpdateCheck { get; set; }

        [ConfigurationKeyName("language")]
        [JsonPropertyName("language")]
        public ApplicationLanguage Language { get; set; }
    }

    public class AppearanceSettings
    {
        [ConfigurationKeyName("theme_mode")]
        [JsonPropertyName("theme_mode")]
        public Theme ThemeMode { get; set; }

        [ConfigurationKeyName("accent_color")]
        [JsonPropertyName("accent_color")]
        public string? AccentColor { get; set; }
    }

    public class LeagueBackupSettings
    {
        [ConfigurationKeyName("backup_tool_path")]
        [JsonPropertyName("backup_tool_path")]
        public string? BackupToolPath { get; set; }

        [ConfigurationKeyName("tool_version")]
        [JsonPropertyName("tool_version")]
        public string? ToolVersion { get; set; }

        [ConfigurationKeyName("repository_path")]
        [JsonPropertyName("repository_path")]
        public string? RepositoryPath { get; set; }

        [ConfigurationKeyName("extract_path")]
        [JsonPropertyName("extract_path")]
        public string? ExtractPath { get; set; }

        [ConfigurationKeyName("available_patches")]
        [JsonPropertyName("available_patches")]
        public string[]? AvailablePatches { get; set; }

        [ConfigurationKeyName("patches_to_keep")]
        [JsonPropertyName("patches_to_keep")]
        public int PatchesToKeep { get; set; } = 1;
    }

    public class ConfigurationFile
    {
        [ConfigurationKeyName("replay_settings")]
        [JsonPropertyName("replay_settings")]
        public ReplaySettings ReplaySettings { get; set; } = new ReplaySettings(); 

        [ConfigurationKeyName("request_settings")]
        [JsonPropertyName("request_settings")]
        public RequestSettings RequestSettings { get; set; } = new RequestSettings();

        [ConfigurationKeyName("general_settings")]
        [JsonPropertyName("general_settings")]
        public GeneralSettings GeneralSettings { get; set; } = new GeneralSettings();

        [ConfigurationKeyName("appearance_settings")]
        [JsonPropertyName("appearance_settings")]
        public AppearanceSettings AppearanceSettings { get; set; } = new AppearanceSettings();

        [ConfigurationKeyName("leaguebackup_settings")]
        [JsonPropertyName("leaguebackup_settings")]
        public LeagueBackupSettings LeagueBackupSettings { get; set; } = new LeagueBackupSettings();

        [ConfigurationKeyName("stash")]
        [JsonPropertyName("stash")]
        public Dictionary<string, object> Stash { get; set; } = new Dictionary<string, object>();
    }
}
