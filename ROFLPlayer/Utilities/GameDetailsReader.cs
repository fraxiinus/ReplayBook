using System.Linq;
using Rofl.Parser;
using ROFLPlayer.Models;

namespace ROFLPlayer.Utilities
{
    public class GameDetailsReader
    {

        /// <summary>
        /// Deduct the map type from game information
        /// </summary>
        /// <param name="replay"></param>
        /// <returns></returns>
        public static Maps GetMapType(ReplayHeader replay)
        {

            // Check if any players have killed jungle creeps, Rules out HA
            var JungleCheck = (from player in replay.MatchMetadata.Players
                               where player["NEUTRAL_MINIONS_KILLED"].ToObject<int>() > 0
                               select player);

            // Check if any players have placed wards, Rules out TT and HA
            var WardCheck = (from player in replay.MatchMetadata.Players
                             where player["WARD_PLACED"].ToObject<int>() > 0
                             select player);

            // Double check between TT and SR
            var DragonCheck = (from player in replay.MatchMetadata.Players
                               where player["DRAGON_KILLS"].ToObject<int>() > 0
                               select player);

            if (JungleCheck.Count() > 0)
            {
                if (WardCheck.Count() == 0 && DragonCheck.Count() == 0)
                {
                    return Maps.TwistedTreeline;
                }
                else
                {
                    return Maps.SummonersRift;
                }

            }
            else
            {
                return Maps.HowlingAbyss;
            }

        }
    }
}
