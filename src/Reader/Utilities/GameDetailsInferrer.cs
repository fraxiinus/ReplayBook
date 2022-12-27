using Fraxiinus.ReplayBook.Reader.Models;
using System.Linq;
using System;
using System.Globalization;

namespace Fraxiinus.ReplayBook.Reader.Utilities
{
    public class GameDetailsInferrer
    {
        public bool InferBlueVictory(Player[] bluePlayers, Player[] redPlayers)
        {
            // If there are blue players
            if(bluePlayers.Count() > 0)
            {
                // Get the first one, and check if they won
                var winText = bluePlayers[0].WIN;
                if(winText.Equals("win", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            // No blue players, are there red players?
            else if(redPlayers.Count() > 0)
            {
                // If so, get the first one and check if they won
                var winText = redPlayers[0].WIN;
                if (winText.Equals("win", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            // Default, blue lost
            return false;
        }

        public MapCode InferMap(Player[] players)
        {
            // Check if any players have killed jungle creeps, Rules out HowlingAbyss
            var JungleCheck = (from player in players
                               where int.Parse(player.NEUTRAL_MINIONS_KILLED, CultureInfo.InvariantCulture) > 0
                               select player);

            // Check if any players have placed wards, Rules out TwistedTreeline and HowlingAbyss
            var WardCheck = (from player in players
                             where int.Parse(player.WARD_PLACED, CultureInfo.InvariantCulture) > 0
                             select player);

            // Double check between TwistedTreeline and SummonersRift
            var DragonCheck = (from player in players
                               where int.Parse(player.DRAGON_KILLS, CultureInfo.InvariantCulture) > 0
                               select player);

            if (JungleCheck.Any())
            {
                if (!WardCheck.Any() && !DragonCheck.Any())
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

        public string GetMapName(MapCode map)
        {
            switch (map)
            {
                case MapCode.HowlingAbyss:
                    return "Howling Abyss";
                case MapCode.SummonersRift:
                    return "Summoner's Rift";
                case MapCode.TwistedTreeline:
                    return "Twisted Treeline";
                default:
                    return "Uknown Map";
            }
        }
    }
}
