using Fraxiinus.Rofl.Extract.Data.Models.Rofl2;

namespace Fraxiinus.ReplayBook.UI.Main.Extensions;

public static class PlayerStatsExtensions
{
    public static string GetPlayerNameOrID(this PlayerStats2 playerStats)
    {
        return string.IsNullOrEmpty(playerStats.Name)
            ? $"N/A - {playerStats.PUUID[^6..]}"
            : playerStats.Name;
    }
}
