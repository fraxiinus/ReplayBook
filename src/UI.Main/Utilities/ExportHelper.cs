namespace Fraxiinus.ReplayBook.UI.Main.Utilities;

using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Fraxiinus.ReplayBook.UI.Main.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using Fraxiinus.ReplayBook.Files.Models;

public static class ExportHelper
{
    private static readonly string _presetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", "export_presets");

    public static string ConstructExportString(ExportDataContext context)
    {
        return context.ExportAsCSV ? ConstructCsvString(context) : ConstructJsonString(context);
    }

    /// <summary>
    /// Returns false if save dialog did not return okay, otherwise return true
    /// </summary>
    /// <param name="context"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static bool ExportToFile(ExportDataContext context, Window parent)
    {
        string results = ConstructExportString(context);

        using var saveDialog = new CommonSaveFileDialog();
        saveDialog.Title = Application.Current.FindResource("ErdExportDialogTitle") as string;
        saveDialog.AddToMostRecentlyUsedList = false;
        saveDialog.EnsureFileExists = true;
        saveDialog.EnsurePathExists = true;
        saveDialog.EnsureReadOnly = false;
        saveDialog.EnsureValidNames = true;
        saveDialog.ShowPlacesList = true;

        saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        saveDialog.DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        saveDialog.DefaultFileName = context.Replay.MatchId + (context.ExportAsCSV ? ".csv" : ".json");

        saveDialog.Filters.Add(context.ExportAsCSV
            ? new CommonFileDialogFilter("CSV Files", "*.csv")
            : new CommonFileDialogFilter("JSON Files", "*.json"));

        // if the dialog did not return ok, return to calling window
        // send parent window as parameter, otherwise it will misplace the popup
        if (saveDialog.ShowDialog(parent) != CommonFileDialogResult.Ok) { return false; }

        string targetFile = saveDialog.FileName;
        File.WriteAllText(targetFile, results);

        // Open the folder and select the file that was made
        _ = Process.Start("explorer.exe", $"/select, \"{targetFile}\"");

        return true;
    }

    public static List<string> FindAllPresets()
    {
        // if path does not exist, return no presets
        // otherwise get all json files and return them
        return !Directory.Exists(_presetPath)
            ? new List<string>()
            : Directory.GetFiles(_presetPath, "*.json").Select(x => Path.GetFileNameWithoutExtension(x)).ToList();
    }

    public static bool PresetNameExists(string name)
    {
        if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name)); }

        // check for invalid file characters
        if (!(name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0)) { throw new ArgumentException("invalid characters"); }

        string filePath = Path.Combine(_presetPath, name + ".json");

        return File.Exists(filePath);
    }

    public static void SavePresetToFile(ExportPreset preset)
    {
        // create the preset path if it doesnt exist
        if (!Directory.Exists(_presetPath)) { _ = Directory.CreateDirectory(_presetPath); }

        string jsonOutput = JsonConvert.SerializeObject(preset, Formatting.Indented);

        File.WriteAllText(Path.Combine(_presetPath, $"{preset.PresetName}.json"), jsonOutput);
    }

    public static ExportPreset LoadPreset(string name)
    {
        string filePath = Path.Combine(_presetPath, name + ".json");

        if (!File.Exists(filePath)) { throw new FileNotFoundException(); }

        string jsonInput = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<ExportPreset>(jsonInput);
    }

    public static void DeletePresetFile(string name)
    {
        string filePath = Path.Combine(_presetPath, name + ".json");

        if (!File.Exists(filePath)) { throw new FileNotFoundException(); }

        File.Delete(filePath);
    }

    private static string ConstructJsonString(ExportDataContext context)
    {
        if (context is null || context.Players is null) { return "{}"; }

        var result = new JObject();

        // add root level properties. Property names based off Riot API
        if (context.IncludeMatchID)
        {
            result["matchId"] = context.Replay.MatchId;
        }
        if (context.IncludeMatchDuration)
        {
            result["gameDuration"] = context.Replay.GameDuration.TotalMilliseconds;
        }
        if (context.IncludePatchVersion)
        {
            result["gameVersion"] = context.Replay.GameVersion;
        }

        // iterate over all player selections
        foreach (ExportPlayerSelectItem playerSelect in context.Players)
        {
            if (playerSelect.Checked)
            {
                // If this is the first player added, include players property first
                if (!result.ContainsKey("participants"))
                {
                    result["participants"] = new JArray();
                }

                var playerAttributes = new JObject();

                // get the player model
                DatabasePlayerStats player = context.Replay.Players
                    .First(x => x.Name.Equals(playerSelect.PlayerPreview.PlayerName, StringComparison.OrdinalIgnoreCase));

                // irate over all attribute selections
                foreach (ExportAttributeSelectItem attributeSelect in context.Attributes)
                {
                    if (attributeSelect.Checked)
                    {
                        string value = player.GetType().GetProperty(attributeSelect.Name).GetValue(player)?.ToString();

                        string attributeName = context.NormalizeAttributeNames ? NormalizeAttributeName(attributeSelect.Name) : attributeSelect.Name;

                        playerAttributes[attributeName] = value;
                    }
                }

                // add players as new jobjects
                (result["participants"] as JArray).Add(playerAttributes);
            }
        }

        return result.ToString(Formatting.Indented);
    }

    private static string ConstructCsvString(ExportDataContext context)
    {
        if (context is null || context.Players is null) { return ""; }

        // Create line for column index
        string index = context.NormalizeAttributeNames ? "player" : "PLAYER";

        // Create dictionary for players, where the key is the player name
        var playerLines = new Dictionary<string, string>();

        foreach (ExportAttributeSelectItem attributeSelect in context.Attributes)
        {
            if (attributeSelect.Checked)
            {
                // add checked attributes to column index
                index += "," + (context.NormalizeAttributeNames ? NormalizeAttributeName(attributeSelect.Name) : attributeSelect.Name);

                // add each player
                foreach (ExportPlayerSelectItem playerSelect in context.Players)
                {
                    if (playerSelect.Checked)
                    {
                        // add the player if its not in the dictionary yet
                        if (!playerLines.ContainsKey(playerSelect.PlayerPreview.PlayerName))
                        {
                            playerLines.Add(playerSelect.PlayerPreview.PlayerName, playerSelect.PlayerPreview.PlayerName);
                        }

                        // get the player object
                        DatabasePlayerStats player = context.Replay.Players
                            .First(x => x.Name.Equals(playerSelect.PlayerPreview.PlayerName, StringComparison.OrdinalIgnoreCase));

                        // Load the attribute value into the player line
                        playerLines[playerSelect.PlayerPreview.PlayerName] += "," + player.GetType().GetProperty(attributeSelect.Name).GetValue(player)?.ToString();
                    }
                }
            }
        }

        foreach (string playerName in playerLines.Keys)
        {
            index += "\n" + playerLines[playerName];
        }

        return index;
    }

    private static string NormalizeAttributeName(string name)
    {
        // lower case all characters
        string value = name.ToLower(CultureInfo.InvariantCulture);

        // remove underscore and capitalize letter after
        int indexOfUnderscore = value.IndexOf('_');
        while (indexOfUnderscore >= 0)
        {
            string capitalizeTarget = value.Substring(indexOfUnderscore + 1, 1).ToUpper(CultureInfo.InvariantCulture);
            value = value.Remove(indexOfUnderscore, 2).Insert(indexOfUnderscore, capitalizeTarget);

            indexOfUnderscore = value.IndexOf('_');
        }

        return value;
    }
}
