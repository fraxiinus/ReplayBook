using Fraxiinus.Rofl.Extract.Data.Models.Rofl2;

namespace Fraxiinus.ReplayBook.UI.Main.Extensions;

public static class PlayerStatsExtensions
{
    public static string GetPlayerNameOrID(this PlayerStats2 playerStats)
    {
        if (!string.IsNullOrEmpty(playerStats.RiotIdGameName) && !string.IsNullOrEmpty(playerStats.RiotIdTagLine))
        {
            return $"{playerStats.RiotIdGameName}#{playerStats.RiotIdTagLine}";
        }
        else if (!string.IsNullOrEmpty(playerStats.Name))
        {
            return playerStats.Name;
        }
        else
        {
            return $"N/A - {playerStats.PUUID[^6..]}";
        }
    }
}
