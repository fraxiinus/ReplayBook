namespace Fraxiinus.ReplayBook.Files.Repositories;

using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.Files.Models;
using Fraxiinus.ReplayBook.Files.Models.Search;
using Fraxiinus.ReplayBook.Files.Utilities;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LuceneDirectory = Lucene.Net.Store.Directory;

public class SearchRepository
{
    private const LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;
    private readonly string _searchDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", "lucene");

    private readonly ObservableConfiguration _config;
    private readonly RiZhi _log;
    // responsible for writing to the index
    private readonly IndexWriter _writer;
    // reponsible for parsing queries
    private readonly QueryParser _queryParser;
    // responsible for searching the index
    private IndexSearcher _searcher;

    // dependencies for lucene, need to be disposed
    private readonly StandardAnalyzer __standardAnalyzer;
    private readonly LuceneDirectory __luceneDirectory;
    private DirectoryReader __directoryReader;

    // variables to support pagination
    private string _queryString;
    private List<SearchResultItem> _searchResults;

    public SearchRepository(ObservableConfiguration config, RiZhi log)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _log = log ?? throw new ArgumentNullException(nameof(log));

        // required objects for lucene
        __standardAnalyzer = new StandardAnalyzer(luceneVersion);
        __luceneDirectory = FSDirectory.Open(_searchDirectory);

        // initialize writer
        var indexConfig = new IndexWriterConfig(luceneVersion, __standardAnalyzer)
        {
            OpenMode = OpenMode.CREATE_OR_APPEND
        };
        _writer = new IndexWriter(__luceneDirectory, indexConfig);

        // initializer reader
        __directoryReader = _writer.GetReader(applyAllDeletes: true);
        _searcher = new IndexSearcher(__directoryReader);

        // create parser
        _queryParser = new QueryParser(luceneVersion, "baseKeywords", __standardAnalyzer);
    }

    ~SearchRepository()
    {
        __directoryReader.Dispose();
        _writer.Dispose();
        __standardAnalyzer.Dispose();
        __luceneDirectory.Dispose();
    }

    /// <summary>
    /// Commit the search repository to disk and update index searcher
    /// </summary>
    public void CommitIndex()
    {
        _writer.Commit();

        // New readers need to be created when index is updated
        __directoryReader = _writer.GetReader(applyAllDeletes: true);
        _searcher = new IndexSearcher(__directoryReader);

        _log.Information("Seach index committed and reader updated");
    }

    /// <summary>
    /// Index a <see cref="FileResult"/> into the search repository
    /// </summary>
    /// <param name="fileResult"></param>
    public void AddDocument(FileResult fileResult)
    {
        var document = new Document
        {
            new StringField("id", fileResult.Id, Field.Store.YES)
        };

        // Default search includes player name with champion
        var playerChampionCombinations = string.Join(", ", fileResult.ReplayFile.Players.Select(p => p.Name + " " + p.Skin));
        // text fields provide full text search, string fields do complete match
        document.Add(new TextField("baseKeywords", playerChampionCombinations, Field.Store.NO));

        // Allow users to search specific teams, allowing a basic matchup search
        var redPlayers = string.Join(", ", fileResult.ReplayFile.RedPlayers.Select(p => p.Name + " " + p.Skin));
        document.Add(new TextField("red", redPlayers, Field.Store.NO));
        var bluePlayers = string.Join(", ", fileResult.ReplayFile.BluePlayers.Select(p => p.Name + " " + p.Skin));
        document.Add(new TextField("blue", bluePlayers, Field.Store.NO));
       
        // These are used for sorting, and must be stored
        document.Add(new StringField("replayName", fileResult.AlternativeName, Field.Store.YES));
        document.Add(new Int64Field("createdDate", fileResult.FileCreationTime.Ticks, Field.Store.YES));
        document.Add(new Int64Field("fileSize", fileResult.FileSizeBytes, Field.Store.YES));

        _writer.AddDocument(document);
    }

    public (IEnumerable<SearchResultItem>, int searchResultCount) Query(SearchParameters searchParameters, int maxEntries, int skip, float minScore = 0.3f, bool forceReset = false)
    {
        if (searchParameters.QueryString != _queryString || forceReset)
        {
            _queryString = searchParameters.QueryString;
            // parse the query and create the collector
            var query = _queryParser.Parse(searchParameters.QueryString);
            var resultsCollector = new ResultsCollector();

            _searcher.Search(query, resultsCollector.GetCollector());

            // return ids that pass the minimum score
            _searchResults = resultsCollector.GetResults(_searcher, minScore);

            // sort results by selected method
            switch (searchParameters.SortMethod)
            {
                case SortMethod.NameAsc:
                    _searchResults = _searchResults.OrderBy(x => x.ReplayName).ToList();
                    break;
                case SortMethod.NameDesc:
                    _searchResults = _searchResults.OrderByDescending(x => x.ReplayName).ToList();
                    break;
                case SortMethod.DateAsc:
                    _searchResults = _searchResults.OrderBy(x => x.CreatedDate).ToList();
                    break;
                case SortMethod.DateDesc:
                    _searchResults = _searchResults.OrderByDescending(x => x.CreatedDate).ToList();
                    break;
                case SortMethod.SizeAsc:
                    _searchResults = _searchResults.OrderBy(x => x.FileSize).ToList();
                    break;
                case SortMethod.SizeDesc:
                    _searchResults = _searchResults.OrderByDescending(x => x.FileSize).ToList();
                    break;
            }
        }

        return (_searchResults.Skip(skip).Take(maxEntries), _searchResults.Count);
    }
}
