namespace Fraxiinus.ReplayBook.Files.Utilities;

using Fraxiinus.ReplayBook.Files.Models.Search;
using Lucene.Net.Search;
using System.Collections.Generic;
using System.Linq;

public class ResultsCollector
{
    private readonly List<(int doc, float score)> _results;
    private Scorer _scorer;
    private int docBase;

    public ResultsCollector()
    {
        _results = new List<(int doc, float score)>();
    }

    public ICollector GetCollector()
    {
        return Collector.NewAnonymous(setScorer: (scorer) =>
        {
            _scorer = scorer;
        }, collect: (doc) =>
        {
            _results.Add((doc + docBase, _scorer.GetScore()));
        }, setNextReader: (context) =>
        {
            docBase = context.DocBase;
        }, acceptsDocsOutOfOrder: () =>
        {
            return true;
        });
    }

    public List<SearchResultItem> GetResults(IndexSearcher searcher, float minScore)
    {
        var returnResults = new List<SearchResultItem>();
        foreach (var (doc, score) in _results.Where(x => x.score > minScore))
        {
            var document = searcher.Doc(doc);

            var searchResult = new SearchResultItem()
            {
                Id = document.Get("id"),
                ReplayName = document.Get("name"),
                CreatedDate = float.Parse(document.Get("createdDate")),
                FileSize = float.Parse(document.Get("fileSize")),
                Score = score
            };

            returnResults.Add(searchResult);
        }

        return returnResults;
    }
}
