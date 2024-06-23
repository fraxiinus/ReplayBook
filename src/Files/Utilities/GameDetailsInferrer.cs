using Fraxiinus.ReplayBook.Files.Models;
using Fraxiinus.Rofl.Extract.Data.Models.Rofl2;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Fraxiinus.ReplayBook.Files.Utilities;
public class GameDetailsInferrer
{
    public static bool InferBlueVictory(IEnumerable<PlayerStats2> bluePlayers, IEnumerable<PlayerStats2> redPlayers)
    {
        // If there are blue players
        if (bluePlayers.Any())
        {
            // Get the first one, and check if they won
            var winText = bluePlayers.First().Win;
            if (winText.Equals("win", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        // No blue players, are there red players?
        else if (redPlayers.Any())
        {
            // If so, get the first one and check if they won
            var winText = redPlayers.First().Win;
            if (winText.Equals("win", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        // Default, blue lost :)
        return false;
    }

    public static MapId InferMap(IEnumerable<PlayerStats2> players)
    {
        // Check if any players have killed jungle creeps, Rules out HowlingAbyss
        var JungleCheck = (from player in players
                           where int.Parse(player.NeutralMinionsKilled, CultureInfo.InvariantCulture) > 0
                           select player);

        // Check if any players have placed wards, Rules out TwistedTreeline and HowlingAbyss
        var WardCheck = (from player in players
                         where int.Parse(player.WardPlaced, CultureInfo.InvariantCulture) > 0
                         select player);

        // Double check between TwistedTreeline and SummonersRift
        var DragonCheck = (from player in players
                           where int.Parse(player.DragonKills, CultureInfo.InvariantCulture) > 0
                           select player);

        if (JungleCheck.Any())
        {
            if (!WardCheck.Any() && !DragonCheck.Any())
            {
                return MapId.TwistedTreeline;
            }
            else
            {
                return MapId.SummonersRift;
            }
        }
        else
        {
            return MapId.HowlingAbyss;
        }
    }

    public static string GetMapName(MapId map) => map switch
    {
        MapId.HowlingAbyss => "Howling Abyss",
        MapId.SummonersRift => "Summoner's Rift",
        MapId.TwistedTreeline => "Twisted Treeline",
        MapId.Unknown => "Unknown",
        _ => "Uknown Map",
    };
}