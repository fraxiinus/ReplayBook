using Rofl.Reader.Models;
using System.Linq;

namespace Rofl.Reader.Utilities
{
    public class GameDetailsInferrer
    {
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
