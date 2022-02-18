//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using Rofl.Reader.Models;
//using Rofl.Reader.Utilities;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Rofl.Reader.Parsers
//{
//    public class LrfParser : IReplayParser
//    {
//        private readonly string _exceptionOriginName = "LrfParser";
//        private readonly byte[] _magicNumbers = new byte[] { 0x00, 0x0B, 0x01, 0x00 };

//        public async Task<ReplayHeader> ReadReplayAsync(FileStream fileStream)
//        {
//            if (!fileStream.CanRead)
//            {
//                throw new IOException($"{_exceptionOriginName} - Stream does not support reading");
//            }

//            // Read and check Magic Numbers
//            byte[] magicbuffer = new byte[4];
//            try
//            {
//                await fileStream.ReadAsync(magicbuffer, 0, 4);
//                if (!magicbuffer.SequenceEqual(_magicNumbers))
//                {
//                    throw new Exception($"{_exceptionOriginName} - Selected file is not in valid LRF format");
//                }
//            }
//            catch (Exception ex)
//            {
//                throw new IOException($"{_exceptionOriginName} - Reading Magic Number: " + ex.Message);
//            }


//            // Read and deserialize length fields
//            // Only parsing length of Match Metadata!!!! All other properties are zero
//            byte[] lengthFieldBuffer = new byte[2];
//            try
//            {
//                await fileStream.ReadAsync(lengthFieldBuffer, 0, 2);
//            }
//            catch (Exception ex)
//            {
//                throw new IOException($"{_exceptionOriginName} - Reading Match Metadata Length Value: " + ex.Message);
//            }

//            var replayLengthField = ParseLengthField(lengthFieldBuffer);


//            // Read and deserialize metadata
//            byte[] metadataBuffer = new byte[replayLengthField.MetadataLength];
//            try
//            {
//                fileStream.Seek(replayLengthField.MetadataOffset, SeekOrigin.Begin);
//                await fileStream.ReadAsync(metadataBuffer, 0, (int)replayLengthField.MetadataLength);
//            }
//            catch (Exception ex)
//            {
//                throw new IOException($"{_exceptionOriginName} - Reading JSON Metadata: " + ex.Message);
//            }

//            var tempResult = ParseMetadata(metadataBuffer);
//            var replayMatchMetadata = tempResult.Item1;
//            var deserializedJson = tempResult.Item2;
                        
//            var replayPayloadFields = ParseMatchHeader(deserializedJson);

//            // Add extra values into the compatility dictionary
//            /*replayMatchMetadata.CompatibilityValues = new Dictionary<string, string>
//            {
//                { "map", (string)deserializedJson["map"] }
//            };*/

//            return new ReplayHeader
//            {
//                LengthFields = replayLengthField,
//                MatchMetadata = replayMatchMetadata,
//                PayloadFields = replayPayloadFields,
//                RawJsonData = JsonConvert.SerializeObject(deserializedJson)
//            };
//        }

//        private static PayloadFields ParseMatchHeader(JObject jsonObject)
//        {
//            var result = new PayloadFields { };

//            result.MatchId = (ulong)jsonObject["matchID"];
//            result.MatchLength = (uint)jsonObject["matchLength"];
//            result.KeyframeAmount = 0;
//            result.ChunkAmount = 0;
//            result.EndChunkID = 0;
//            result.StartChunkID = 0;
//            result.KeyframeInterval = 0;
//            result.EncryptionKeyLength = 0;
//            result.EncryptionKey = (string)jsonObject["encryptionKey"];

//            return result;
//        }

//        private static (MatchMetadata, JObject) ParseMetadata(byte[] bytedata)
//        {
//            var result = new MatchMetadata { };
//            var jsonstring = Encoding.UTF8.GetString(bytedata);

//            var jsonobject = JObject.Parse(jsonstring);

//            result.GameDuration = ((ulong)jsonobject["matchLength"] * 1000);
//            result.GameVersion = (string)jsonobject["clientVersion"];
//            result.LastGameChunkID = 0;
//            result.LastKeyframeID = 0;

//            // Create new lists of player dictionaries for sorting
//            var blueTeam = new List<Dictionary<string, string>>();
//            var redTeam = new List<Dictionary<string, string>>();

//            // Sort blue and red teams
//            foreach (JObject player in (JArray)jsonobject["players"])
//            {
//                if (player["team"].ToString() == "1")
//                {
//                    var playerDict = player.ToObject<Dictionary<string, string>>();
//                    playerDict = TranslatePlayerDictionary(playerDict, (string)jsonobject["map"]);
//                    blueTeam.Add(playerDict);
//                }
//                else if (player["team"].ToString() == "2")
//                {
//                    var playerDict = player.ToObject<Dictionary<string, string>>();
//                    playerDict = TranslatePlayerDictionary(playerDict, (string)jsonobject["map"]);
//                    redTeam.Add(playerDict);
//                }
//            }

//            result.BluePlayers = blueTeam.ToArray();
//            result.RedPlayers = redTeam.ToArray();

//            //result.Players = JArray.Parse(((string)jsonobject["statsJson"]).Replace(@"\", ""));

//            return (result, jsonobject);
//        }

//        private static Dictionary<string, string> TranslatePlayerDictionary(Dictionary<string, string> player, string mapId)
//        {
            
//            player.Add("WARD_PLACED", "0");
//            player.Add("DRAGON_KILLS", "0");

//            if(mapId == "11" || mapId == "-1")
//            {
//                player["WARD_PLACED"] = "1";
//            }
            
//            player.Add("NEUTRAL_MINIONS_KILLED", player.SafeGet("neutralMinionsKilled"));

//            player.Add("WIN", Boolean.Parse(player.SafeGet("won")) ? "WIN" : "FAIL");

//            player.Add("SKIN", player.SafeGet("champion"));

//            player.Add("NAME", String.IsNullOrEmpty(player.SafeGet("summoner")) ? player.SafeGet("champion") : player.SafeGet("summoner"));

//            player.Add("ITEM0", player.SafeGet("item1"));
//            player.Add("ITEM1", player.SafeGet("item2"));
//            player.Add("ITEM2", player.SafeGet("item3"));
//            player.Add("ITEM3", player.SafeGet("item4"));
//            player.Add("ITEM4", player.SafeGet("item5"));
//            player.Add("ITEM5", player.SafeGet("item6"));
//            player.Add("ITEM6", player.SafeGet("item7"));

//            player.Add("LEVEL", player.SafeGet("level"));

//            player.Add("CHAMPIONS_KILLED", player.SafeGet("kills"));
//            player.Add("NUM_DEATHS", player.SafeGet("deaths"));
//            player.Add("ASSISTS", player.SafeGet("assists"));
//            player.Add("MINIONS_KILLED", player.SafeGet("minions"));
//            player.Add("GOLD_EARNED", player.SafeGet("gold"));
//            player.Add("GOLD_SPENT", "0");
//            player.Add("TURRETS_KILLED", "0");
//            player.Add("TOTAL_DAMAGE_DEALT_TO_CHAMPIONS", "0");
//            player.Add("TOTAL_DAMAGE_DEALT_TO_OBJECTIVES", "0");
//            player.Add("TOTAL_DAMAGE_DEALT_TO_TURRETS", "0");
//            player.Add("TOTAL_DAMAGE_DEALT", player.SafeGet("damageDealt"));
//            player.Add("TOTAL_HEAL", player.SafeGet("healed"));
//            player.Add("TOTAL_DAMAGE_TAKEN", player.SafeGet("damageTaken"));
//            player.Add("VISION_SCORE", "0");

//            return player;
//        }

//        private static LengthFields ParseLengthField(byte[] bytedata)
//        {
//            var result = new LengthFields { };
//            result.HeaderLength = 0;
//            result.FileLength = 0;
//            result.PayloadHeaderOffset = 0;
//            result.PayloadHeaderLength = 0;
//            result.PayloadOffset = 0;

//            result.MetadataOffset = 8;
//            result.MetadataLength = BitConverter.ToUInt16(bytedata, 0);

//            return result;
//        }
//    }
//}
