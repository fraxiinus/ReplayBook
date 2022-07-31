using Fraxiinus.Rofl.Extract.Data.Models;
using System.Globalization;
using System.Linq;

namespace Fraxiinus.ReplayBook.Files.Utilities;

public enum MapCode
{
    TwistedTreeline = 10, SummonersRift = 11, HowlingAbyss = 12, Unknown = 99
}

public static class ROFLExtensions
{
    public static bool CalculateBlueTeamVictory(this ROFL replayFile)
    {
        var bluePlayer = replayFile.Metadata.PlayerStatistics.FirstOrDefault(x => x.Team == "100");

        // if blue player do not exist,
        // or blue player did not win
        if (bluePlayer == null || 
            !bluePlayer.Win.Equals("WIN", System.StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static MapCode GuessMap(this ROFL replayFile)
    {
        // Check for jungle kills, howling abyss has no neutral minions
        var JungleCheck = replayFile.Metadata.PlayerStatistics
            .Where(player => int.TryParse(player.NeutralMinionsKilled,
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out int result)
                && result > 0);
        // Check for placed wards, twisted treeline and howling abyss do not allow wards
        var WardCheck = replayFile.Metadata.PlayerStatistics
            .Where(player => int.TryParse(player.WardPlaced,
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out int result)
                && result > 0);
        // twisted treeline does not have dragon
        var DragonCheck = replayFile.Metadata.PlayerStatistics
            .Where(player => int.TryParse(player.WardPlaced,
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out int result)
                && result > 0);

        if (JungleCheck.Any())
        {
            if(!WardCheck.Any() && !DragonCheck.Any())
            {
                return MapCode.TwistedTreeline;
            }
            else
            {
                return MapCode.SummonersRift;
            }
        }
        else
        {
            return MapCode.HowlingAbyss;
        }
    }
}
