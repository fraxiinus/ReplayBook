namespace Fraxiinus.ReplayBook.Files.Models;

using Fraxiinus.ReplayBook.Files.Utilities;
using Fraxiinus.Rofl.Extract.Data.Models;
using Fraxiinus.Rofl.Extract.Data.Models.Rofl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ReplayFile
{
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
                var z = y as DatabasePlayerStats;
                z.DatabaseId = $"{MatchId}_{y.Id}";
                return z;
            });
        RedPlayers = input.Metadata.PlayerStatistics
            .Where(x => x.Team == "200")
            .Select(y =>
            {
                var z = y as DatabasePlayerStats;
                z.DatabaseId = $"{MatchId}_{y.Id}";
                return z;
            });

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

    public IEnumerable<DatabasePlayerStats> Players
    {
        get => BluePlayers.Union(RedPlayers);
    }

    public IEnumerable<DatabasePlayerStats> BluePlayers { get; set; }

    public IEnumerable<DatabasePlayerStats> RedPlayers { get; set; }

    // Inferred fields
    public MapId MapId { get; set; }

    public string MapName { get; set; }

    public bool IsBlueVictorious { get; set; }

    // User assigned fields
    public string AlternativeName { get; set; }
}
