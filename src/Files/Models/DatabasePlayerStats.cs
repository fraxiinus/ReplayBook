namespace Fraxiinus.ReplayBook.Files.Models;

using Fraxiinus.Rofl.Extract.Data.Models.Rofl;

/// <summary>
/// Provide compatibility with database scheme
/// </summary>
public class DatabasePlayerStats : PlayerStats
{
    public string DatabaseId { get; set; }
}
