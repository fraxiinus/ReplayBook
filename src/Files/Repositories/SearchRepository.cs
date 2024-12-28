namespace Fraxiinus.ReplayBook.Files.Repositories;

using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.Files.Models;
using Fraxiinus.ReplayBook.Files.Models.Search;
using Fraxiinus.ReplayBook.Files.Utilities;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Miscellaneous;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Flexible.Standard;
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
    private readonly StandardQueryParser _queryParser;
    // responsible for searching the index
    private IndexSearcher _searcher;

    // dependencies for lucene, need to be disposed
    private readonly PerFieldAnalyzerWrapper __analyzer;
    private readonly LuceneDirectory __luceneDirectory;
    private DirectoryReader __directoryReader;

    // variables to support pagination
    private string _queryString;
    private List<SearchResultItem> _searchResults;

    public SearchRepository(ObservableConfiguration config, RiZhi log)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _log = log ?? throw new ArgumentNullException(nameof(log));

        // Analyzers used for fields
        var analyzersPerField = new Dictionary<string, Analyzer>
        {   // some fields need different analyzers in order to query properly
            ["name"] = new KeywordAnalyzer(),
            ["fileSize"] = new KeywordAnalyzer(),
            ["id"] = new KeywordAnalyzer()
        };
        __analyzer = new PerFieldAnalyzerWrapper(new StandardAnalyzer(luceneVersion), analyzersPerField);

        __luceneDirectory = FSDirectory.Open(_searchDirectory);
        // initialize writer
        var indexConfig = new IndexWriterConfig(luceneVersion, __analyzer)
        {
            OpenMode = OpenMode.CREATE_OR_APPEND
        };
        _writer = new IndexWriter(__luceneDirectory, indexConfig);
        _writer.Commit(); // commit index in case the index was just created

        // initializer reader
        __directoryReader = DirectoryReader.Open(__luceneDirectory);
                            //_writer.GetReader(applyAllDeletes: true);
        _searcher = new IndexSearcher(__directoryReader);

        // create parser and required configurations
        //var numericConfigMap = new Dictionary<string, NumericConfig>
        //{   // numeric fields need definitions on how to compare numbers
        //    ["length"] = new NumericConfig(1, new SimpleNumberFormat(CultureInfo.InvariantCulture), NumericType.INT32)
        //};
        //var dateResolutionMap = new Dictionary<string, DateResolution>
        //{   // date fields need resolution definitions
        //    ["date"] = DateResolution.DAY,
        //};
        _queryParser = new StandardQueryParser(__analyzer)
        {
            //NumericConfigMap = numericConfigMap,
            //DateResolutionMap = dateResolutionMap
        };
    }

    ~SearchRepository()
    {
        __directoryReader.Dispose();
        _writer.Dispose();
        __analyzer.Dispose();
        __luceneDirectory.Dispose();
    }

    public string SearchIndexDirectory => _searchDirectory;

    /// <summary>
    /// Commit the search repository to disk and update index searcher
    /// </summary>
    public void CommitIndex()
    {
        // Writes to disk and updates index
        _writer.Commit();

        // New readers need to be created when index is updated
        __directoryReader = DirectoryReader.OpenIfChanged(__directoryReader) ?? __directoryReader;
        _searcher = new IndexSearcher(__directoryReader);

        _log.Information("Search index committed and reader updated");
    }

    public void DeleteIndex()
    {
        _writer.DeleteAll();

        _writer.Commit();

        _log.Information("Search index deleted and committed");

        __directoryReader.Dispose();
        _writer.Dispose();
        __analyzer.Dispose();
        __luceneDirectory.Dispose();
    }

    /// <summary>
    /// Index a <see cref="FileResult"/> into the search repository
    /// </summary>
    /// <param name="fileResult"></param>
    public void AddDocument(FileResult fileResult)
    {
        var document = CreateDocument(fileResult);

        _writer.AddDocument(document);
    }

    public void RemoveDocument(string id)
    {
        _writer.DeleteDocuments(new Term("id", id));
    }

    /// <summary>
    /// This action commits the index!
    /// </summary>
    /// <param name="fileResult"></param>
    /// <param name="newName"></param>
    public void UpdateDocumentName(FileResult fileResult, string newName)
    {
        fileResult.AlternativeName = newName;
        var document = CreateDocument(fileResult);
        _writer.UpdateDocument(new Term("id", fileResult.Id), document);
        CommitIndex();
    }

    public (IEnumerable<SearchResultItem>, int searchResultCount) Query(SearchParameters searchParameters, int maxEntries, int skip, float minScore = 0.3f, bool forceReset = false)
    {
        if (searchParameters.QueryString != _queryString || forceReset)
        {
            _queryString = QueryParserUtil.Escape(searchParameters.QueryString);
            // parse the query and create the collector
            var query = _queryParser.Parse(searchParameters.QueryString, "baseKeywords");
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

    private static Document CreateDocument(FileResult fileResult)
    {
        var document = new Document
        {
            new StringField("id", fileResult.Id, Field.Store.YES)
        };

        if (fileResult.ReplayFile != default)
        {
            // Default search includes player name with champion
            var playerChampionCombinations = string.Join(", ", fileResult.ReplayFile.Players.Select(p => p.Name + " " + GetSafeChampionName(p.Skin)));
            // text fields provide full text search, string fields do complete match
            document.Add(new TextField("baseKeywords", fileResult.AlternativeName + ", " + playerChampionCombinations, Field.Store.NO));

            // Allow users to search specific teams, allowing a basic matchup search
            var redPlayers = string.Join(", ", fileResult.ReplayFile.RedPlayers.Select(p => p.Name + " " + GetSafeChampionName(p.Skin)));
            var bluePlayers = string.Join(", ", fileResult.ReplayFile.BluePlayers.Select(p => p.Name + " " + GetSafeChampionName(p.Skin)));
            document.Add(new TextField("red", redPlayers, Field.Store.NO));
            document.Add(new TextField("blue", bluePlayers, Field.Store.NO));
            
            // enable game length query
            document.Add(new StringField("length", $"{fileResult.ReplayFile.GameDuration.Minutes}{fileResult.ReplayFile.GameDuration.Seconds}", Field.Store.NO));
        }

        // Query date, allow for date range query
        document.Add(new StringField("date", DateTools.DateToString(fileResult.FileCreationTime, DateResolution.DAY), Field.Store.NO));
        // These are used for sorting, and must be stored
        document.Add(new StringField("name", fileResult.AlternativeName, Field.Store.YES));
        document.Add(new Int64Field("createdDate", fileResult.FileCreationTime.Ticks, Field.Store.YES));
        document.Add(new Int64Field("fileSize", fileResult.FileSizeBytes, Field.Store.YES));

        return document;
    }

    /// <summary>
    /// Spaces are added, symbols removed, and monkeyking renamed
    /// </summary>
    /// <param name="championName"></param>
    /// <returns></returns>
    private static string GetSafeChampionName(string championName)
    {
        return championName switch
        {
            "AurelionSol" => "Aurelion Sol",
            "DrMundo" => "Dr Mundo",
            "JarvanIV" => "Jarvan",
            "LeeSin" => "Lee Sin",
            "MasterYi" => "Master Yi",
            "MissFortune" => "Miss Fortune",
            "MonkeyKing" => "Wukong",
            "Nunu" => "Nunu Willump",
            "Renata" => "Renata Glasc",
            "TahmKench" => "Tahm Kench",
            "TwistedFate" => "Twisted Fate",
            "XinZhao" => "Xin Zhao",
            _ => championName,
        };
    }
}
