namespace Fraxiinus.ReplayBook.Files.Models.Search;

public class SearchResultItem
{
    public string Id { get; set; }

    public string ReplayName { get; set; }

    public float CreatedDate { get; set; }

    public float FileSize { get; set; }

    public float Score { get; set; }
}
