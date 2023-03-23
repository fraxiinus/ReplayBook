namespace Fraxiinus.ReplayBook.Files.Utilities;

using Fraxiinus.ReplayBook.Files.Models.Search;
using Lucene.Net.Search;
using System.Collections.Generic;
using System.Linq;

public class ResultsCollector
{
    private List<(int doc, float score)> _results;
    // private (int doc, float score) _stagingResult;
    private Scorer _scorer;

    public ResultsCollector()
    {
        _results = new List<(int doc, float score)>();
        //_stagingResult.doc = -1;
    }

    public ICollector GetCollector()
    {
        return Collector.NewAnonymous(setScorer: (scorer) =>
        {
            _scorer = scorer;
        }, collect: (doc) =>
        {
            _results.Add((doc, _scorer.GetScore()));
        }, setNextReader: (context) =>
        {
            //
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
                ReplayName = document.Get("replayName"),
                CreatedDate = float.Parse(document.Get("createdDate")),
                FileSize = float.Parse(document.Get("fileSize")),
                Score = score
            };

            returnResults.Add(searchResult);
        }

        return returnResults;
    }

    //private void GoNextResult()
    //{
    //    if (_stagingResult.doc != -1)
    //    {
    //        _results.Add(_stagingResult);
    //    }
    //}
}
