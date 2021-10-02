using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rofl.Reader.Models;
using Rofl.UI.Main.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;

namespace Rofl.UI.Main.Utilities
{
    public static class ExportHelper
    {
        private static readonly string _presetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", "export_presets");

        public static string ConstructExportString(ExportDataContext context)
        {
            return context.ExportAsCSV ? ConstructCsvString(context) : ConstructJsonString(context);
        }

        public static bool ExportToFile(ExportDataContext context, Window parent)
        {
            string results = ConstructExportString(context);

            using (CommonSaveFileDialog saveDialog = new CommonSaveFileDialog())
            {
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

        private static string ConstructJsonString(ExportDataContext context)
        {
            if (context is null || context.Players is null) { return "{}"; }

            JObject result = new JObject();

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

                    JObject playerAttributes = new JObject();

                    // get the player model
                    Player player = context.Replay.Players.First(x => x.NAME.Equals(playerSelect.PlayerPreview.PlayerName, StringComparison.OrdinalIgnoreCase));

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

            return result.ToString(Newtonsoft.Json.Formatting.Indented);
        }

        private static string ConstructCsvString(ExportDataContext context)
        {
            if (context is null || context.Players is null) { return ""; }

            // Create line for column index
            string index = context.NormalizeAttributeNames ? "player" : "PLAYER";

            // Create dictionary for players, where the key is the player name
            Dictionary<string, string> playerLines = new Dictionary<string, string>();

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
                            Player player = context.Replay.Players.First(x => x.NAME.Equals(playerSelect.PlayerPreview.PlayerName, StringComparison.OrdinalIgnoreCase));

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


        /// Horrible old functions:

        public static string ConstructJsonString(ReplayFile replay, List<ExportSelectItem> LevelOneItems, List<ExportSelectItem> LevelTwoItems, List<ExportSelectItem> LevelThreeItems)
        {
            if (replay == null) { throw new ArgumentNullException(nameof(replay)); }
            if (LevelOneItems == null) { throw new ArgumentNullException(nameof(LevelOneItems)); }
            if (LevelTwoItems == null) { throw new ArgumentNullException(nameof(LevelTwoItems)); }
            if (LevelThreeItems == null) { throw new ArgumentNullException(nameof(LevelThreeItems)); }

            JObject result = new JObject();

            JsonSerializeLevelOne(result, LevelOneItems);

            JsonSerializeLevelTwo(result, LevelTwoItems);

            JsonSerializeLevelThree(result, replay, LevelThreeItems);

            return result.ToString(Newtonsoft.Json.Formatting.Indented);
        }

        public static string ConstructCsvString(ReplayFile replay, List<ExportSelectItem> LevelTwoItems, List<ExportSelectItem> LevelThreeItems)
        {
            if (replay == null) { throw new ArgumentNullException(nameof(replay)); }
            if (LevelTwoItems == null) { throw new ArgumentNullException(nameof(LevelTwoItems)); }
            if (LevelThreeItems == null) { throw new ArgumentNullException(nameof(LevelThreeItems)); }

            List<string> lines = new List<string>();
            bool doneOnce = false;

            // Add empty line for column index
            lines.Add("PLAYER");

            // Create enough strings for all the players
            foreach (ExportSelectItem playerName in LevelTwoItems)
            {
                if (!playerName.Checked) { continue; }

                // Get the player in question
                Player player = replay.Players.First(x => x.NAME.Equals(playerName.Name, StringComparison.OrdinalIgnoreCase));
                string playerString = playerName.Name;

                // Load property values for player
                foreach (ExportSelectItem prop in LevelThreeItems)
                {
                    if (!prop.Checked) { continue; }

                    // Add property name
                    if (!doneOnce)
                    {
                        lines[0] += "," + prop.Name;
                    }

                    // Add property value to player
                    playerString += "," + player.GetType().GetProperty(prop.Name).GetValue(player)?.ToString();
                }

                doneOnce = true; // do not add props to the index more than once
                lines.Add(playerString);
            }

            return string.Join("\n", lines);
        }

        #region JSON Helper Fields

        private static void JsonSerializeLevelOne(JObject result, List<ExportSelectItem> selectItems)
        {
            foreach (ExportSelectItem item in selectItems)
            {
                if (!item.Checked) { continue; }

                // Include players field for level two to see
                if (item.Name.Equals("Players", StringComparison.OrdinalIgnoreCase))
                {
                    result[item.Name] = new JArray();
                    continue;
                }

                result[item.Name] = item.Value;
            }
        }

        private static void JsonSerializeLevelTwo(JObject result, List<ExportSelectItem> selectItems)
        {
            // if there is no player property, we cant populate it
            if (!result.ContainsKey("Players")) { return; }

            foreach (ExportSelectItem item in selectItems)
            {
                if (!item.Checked) { continue; }

                // Add the player name we need to add in level three
                (result["Players"] as JArray).Add(item.Name);
            }
        }

        private static void JsonSerializeLevelThree(JObject result, ReplayFile replay, List<ExportSelectItem> selectItems)
        {
            // if there is no player property, we cant populate it
            if (!result.ContainsKey("Players")) { return; }

            // If there are no players, we cannot populate them
            if (!result["Players"].Any()) { return; }

            JArray populatedPlayers = new JArray();
            foreach (JToken playerName in result["Players"])
            {
                // Get the player in question
                Player player = replay.Players.First(x => x.NAME.Equals(playerName.ToString(), StringComparison.OrdinalIgnoreCase));

                JObject jsonPlayer = new JObject();
                foreach (ExportSelectItem item in selectItems)
                {
                    if (!item.Checked) { continue; }

                    // Get the real value
                    string value = player.GetType().GetProperty(item.Name).GetValue(player)?.ToString();
                    jsonPlayer[item.Name] = value;
                }

                // Add player to array
                populatedPlayers.Add(jsonPlayer);
            }

            result["Players"] = populatedPlayers;
        }

        #endregion

    }
}
