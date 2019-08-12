using Rofl.Reader.Models;
using System.Linq;
using System;

namespace Rofl.Reader.Utilities
{
    public class GameDetailsInferrer
    {
        public bool InferBlueVictory(MatchMetadata matchMetadata)
        {
            // If there are blue players
            if(matchMetadata.BluePlayers.Count() > 0)
            {
                // Get the first one, and check if they won
                var winText = matchMetadata.BluePlayers[0].SafeGet("WIN");
                if(winText.Equals("win", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            // No blue players, are there red players?
            else if(matchMetadata.RedPlayers.Count() > 0)
            {
                // If so, get the first one and check if they won
                var winText = matchMetadata.RedPlayers[0].SafeGet("WIN");
                if (winText.Equals("win", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            // Default, blue lost
            return false;
        }

        public Map InferMap(MatchMetadata matchMetadata)
        {
            // Check if any players have killed jungle creeps, Rules out HowlingAbyss
            var JungleCheck = (from player in matchMetadata.AllPlayers
                               where int.Parse(player["NEUTRAL_MINIONS_KILLED"]) > 0
                               select player);

            // Check if any players have placed wards, Rules out TwistedTreeline and HowlingAbyss
            var WardCheck = (from player in matchMetadata.AllPlayers
                             where int.Parse(player["WARD_PLACED"]) > 0
                             select player);

            // Double check between TwistedTreeline and SummonersRift
            var DragonCheck = (from player in matchMetadata.AllPlayers
                               where int.Parse(player["DRAGON_KILLS"]) > 0
                               select player);

            if (JungleCheck.Count() > 0)
            {
                if (WardCheck.Count() == 0 && DragonCheck.Count() == 0)
                {
                    return Map.TwistedTreeline;
                }
                else
                {
                    return Map.SummonersRift;
                }

            }
            else
            {
                return Map.HowlingAbyss;
            }
        }
    }
}
