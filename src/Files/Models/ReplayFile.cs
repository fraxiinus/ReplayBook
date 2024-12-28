namespace Fraxiinus.ReplayBook.Files.Models;

using Fraxiinus.ReplayBook.Files.Utilities;
using Fraxiinus.Rofl.Extract.Data;
using Fraxiinus.Rofl.Extract.Data.Models;
using Fraxiinus.Rofl.Extract.Data.Models.Rofl2;
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

    public ReplayFile(string fullFilePath, ParseResult input)
    {
        Name = Path.GetFileName(fullFilePath);
        // AlternativeName = Name;
        Location = fullFilePath;

        Type = input.Type;
        switch (Type)
        {
            case ReplayType.ROFL2:
                LoadFromROFL2File((ROFL2)input.Result);
                break;
            case ReplayType.ROFL:
                LoadFromROFLFile((ROFL)input.Result);
                break;
        }

        // Infer values
        MapId = GameDetailsInferrer.InferMap(Players);
        MapName = GameDetailsInferrer.GetMapName(MapId);
        IsBlueVictorious = GameDetailsInferrer.InferBlueVictory(BluePlayers, RedPlayers);
    }

    private void LoadFromROFL2File(ROFL2 input)
    {
        // Copy values
        GameDuration = TimeSpan.FromMilliseconds(input.Metadata.GameLength);
        GameVersion = input.Metadata.GameVersion;
        MatchId = "Unknown";

        // UniqueId must be unique for every player in a match.
        // It is used to optimize the player cache so the same object isn't loaded twice

        BluePlayers = input.Metadata.PlayerStatistics
            .Where(x => x.Team == "100")
            .Select(y =>
            {
                y.UniqueId = $"{y.Id}_{y.Exp}_{GameDuration}";
                return y;
            }).ToList();
        RedPlayers = input.Metadata.PlayerStatistics
            .Where(x => x.Team == "200")
            .Select(y =>
            {
                y.UniqueId = $"{y.Id}_{y.Exp}_{GameDuration}";
                return y;
            }).ToList();
    }

    private void LoadFromROFLFile(ROFL input)
    {
        // Copy values
        GameDuration = TimeSpan.FromMilliseconds(input.Metadata.GameLength);
        GameVersion = input.Metadata.GameVersion;
        MatchId = input.PayloadHeader.GameId.ToString();

        BluePlayers = input.Metadata.PlayerStatistics
            .Where(x => x.Team == "100")
            .Select(y =>
            {
                y.UniqueId = $"{y.Id}_{y.Exp}_{GameDuration}";
                return RoflBaseClassConverter.ToPlayerStats2(y);
            }).ToList();

        RedPlayers = input.Metadata.PlayerStatistics
            .Where(x => x.Team == "200")
            .Select(y =>
            {
                y.UniqueId = $"{y.Id}_{y.Exp}_{GameDuration}";
                return RoflBaseClassConverter.ToPlayerStats2(y);
            }).ToList();
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
    public IEnumerable<PlayerStats2> Players
    {
        get => BluePlayers.Union(RedPlayers);
    }

    public List<PlayerStats2> BluePlayers { get; set; }

    public List<PlayerStats2> RedPlayers { get; set; }

    public ReplayType Type { get; set; }

    // Inferred fields
    public MapId MapId { get; set; }

    public string MapName { get; set; }

    public bool IsBlueVictorious { get; set; }

    // User assigned fields
    //public string AlternativeName { get; set; }
}
