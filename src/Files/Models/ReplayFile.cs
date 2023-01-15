namespace Fraxiinus.ReplayBook.Files.Models;

using Fraxiinus.ReplayBook.Files.Utilities;
using Fraxiinus.Rofl.Extract.Data.Models;
using Fraxiinus.Rofl.Extract.Data.Models.Rofl;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ReplayFile
{
    /// <summary>
    /// Blank constructor for LiteDB
    /// </summary>
    public ReplayFile() { }

    public ReplayFile(string fullFilePath, ROFL input)
    {
        Name = Path.GetFileName(fullFilePath);
        AlternativeName = Name;
        Location = fullFilePath;

        // Copy values
        GameDuration = TimeSpan.FromMilliseconds(input.Metadata.GameLength);
        GameVersion = input.Metadata.GameVersion;
        MatchId = input.PayloadHeader.GameId.ToString();

        BluePlayers = input.Metadata.PlayerStatistics
            .Where(x => x.Team == "100")
            .Select(y =>
            {
                y.UniqueId = $"{MatchId}_{y.Id}";
                return y;
            }).ToList();
        RedPlayers = input.Metadata.PlayerStatistics
            .Where(x => x.Team == "200")
            .Select(y =>
            {
                y.UniqueId = $"{MatchId}_{y.Id}";
                return y;
            }).ToList();

        // Infer values
        MapId = GameDetailsInferrer.InferMap(Players);
        MapName = GameDetailsInferrer.GetMapName(MapId);
        IsBlueVictorious = GameDetailsInferrer.InferBlueVictory(BluePlayers, RedPlayers);
    }

    /// <summary>
    /// Name of the file
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Full path of the file
    /// </summary>
    public string Location { get; set; }

    public TimeSpan GameDuration { get; set; }

    public string GameVersion { get; set; }

    public string MatchId { get; set; }

    [BsonIgnore]
    public IEnumerable<PlayerStats> Players
    {
        get => BluePlayers.Union(RedPlayers);
    }

    public List<PlayerStats> BluePlayers { get; set; }

    public List<PlayerStats> RedPlayers { get; set; }

    // Inferred fields
    public MapId MapId { get; set; }

    public string MapName { get; set; }

    public bool IsBlueVictorious { get; set; }

    // User assigned fields
    public string AlternativeName { get; set; }
}
