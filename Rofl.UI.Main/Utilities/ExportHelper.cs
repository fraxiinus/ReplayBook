using Newtonsoft.Json.Linq;
using Rofl.Reader.Models;
using Rofl.UI.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.UI.Main.Utilities
{
    public static class ExportHelper
    {
        public static string ConstructJsonString(ReplayFile replay, List<ExportSelectItem> LevelOneItems, List<ExportSelectItem> LevelTwoItems, List<ExportSelectItem> LevelThreeItems)
        {
            if (replay == null) throw new ArgumentNullException(nameof(replay));
            if (LevelOneItems == null) throw new ArgumentNullException(nameof(LevelOneItems));
            if (LevelTwoItems == null) throw new ArgumentNullException(nameof(LevelTwoItems));
            if (LevelThreeItems == null) throw new ArgumentNullException(nameof(LevelThreeItems));

            JObject result = new JObject();

            JsonSerializeLevelOne(result, LevelOneItems);

            JsonSerializeLevelTwo(result, LevelTwoItems);

            JsonSerializeLevelThree(result, replay, LevelThreeItems);

            return result.ToString(Newtonsoft.Json.Formatting.Indented);
        }

        public static string ConstructCsvString(ReplayFile replay, List<ExportSelectItem> LevelTwoItems, List<ExportSelectItem> LevelThreeItems)
        {
            if (replay == null) throw new ArgumentNullException(nameof(replay));
            if (LevelTwoItems == null) throw new ArgumentNullException(nameof(LevelTwoItems));
            if (LevelThreeItems == null) throw new ArgumentNullException(nameof(LevelThreeItems));

            List<string> lines = new List<string>();
            bool doneOnce = false;

            // Add empty line for column index
            lines.Add("PLAYER");

            // Create enough strings for all the players
            foreach (var playerName in LevelTwoItems)
            {
                if (!playerName.Checked) continue;

                // Get the player in question
                var player = replay.Players.First(x => x.NAME.Equals(playerName.Name, StringComparison.OrdinalIgnoreCase));
                var playerString = playerName.Name;

                // Load property values for player
                foreach (var prop in LevelThreeItems)
                {
                    if (!prop.Checked) continue;

                    // Add property name
                    if (!doneOnce)
                    {
                        lines[0] += "," + prop.Name;
                    }

                    // Add property value to player
                    playerString += "," + player.GetType().GetProperty(prop.Name).GetValue(player)?.ToString(); ;
                }

                doneOnce = true; // do not add props to the index more than once
                lines.Add(playerString);
            }

            return String.Join("\n", lines);
        }

        #region JSON Helper Fields

        private static void JsonSerializeLevelOne(JObject result, List<ExportSelectItem> selectItems)
        {
            foreach (var item in selectItems)
            {
                if (!item.Checked) continue;

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
            if (!result.ContainsKey("Players")) return;

            foreach (var item in selectItems)
            {
                if (!item.Checked) continue;

                // Add the player name we need to add in level three
                (result["Players"] as JArray).Add(item.Name);
            }
        }

        private static void JsonSerializeLevelThree(JObject result, ReplayFile replay, List<ExportSelectItem> selectItems)
        {
            // if there is no player property, we cant populate it
            if (!result.ContainsKey("Players")) return;

            // If there are no players, we cannot populate them
            if (!result["Players"].Any()) return;

            JArray populatedPlayers = new JArray();
            foreach (var playerName in result["Players"])
            {
                // Get the player in question
                var player = replay.Players.First(x => x.NAME.Equals(playerName.ToString(), StringComparison.OrdinalIgnoreCase));

                JObject jsonPlayer = new JObject();
                foreach (var item in selectItems)
                {
                    if (!item.Checked) continue;

                    // Get the real value
                    var value = player.GetType().GetProperty(item.Name).GetValue(player)?.ToString();
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
