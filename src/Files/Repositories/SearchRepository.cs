namespace Fraxiinus.ReplayBook.Files.Repositories;

using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.Files.Models;
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
    private readonly IndexWriter _writer;
    private readonly QueryParser _queryParser;
    private IndexSearcher _searcher;

    // dependencies for lucene, need to be disposed
    private readonly StandardAnalyzer __standardAnalyzer;
    private readonly LuceneDirectory __luceneDirectory;
    private DirectoryReader __directoryReader;

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
        _queryParser = new QueryParser(luceneVersion, "keywords", __standardAnalyzer);
    }

    ~SearchRepository()
    {
        __directoryReader.Dispose();
        _writer.Dispose();
        __standardAnalyzer.Dispose();
        __luceneDirectory.Dispose();
    }

    public void CommitIndex()
    {
        _writer.Commit();

        // New readers need to be created when index is updated
        __directoryReader = _writer.GetReader(applyAllDeletes: true);
        _searcher = new IndexSearcher(__directoryReader);

        _log.Information("Seach index committed and reader updated");
    }

    public void AddDocument(FileResult fileResult)
    {
        var document = new Document
        {
            new TextField("id", fileResult.Id, Field.Store.YES)
        };

        var playerNames = string.Join(", ", fileResult.ReplayFile.Players.Select(p => p.Name));
        var championNames = string.Join(", ", fileResult.ReplayFile.Players.Select(p => p.Skin));
        // text fields provide full text search, string fields do complete match
        document.Add(new TextField("keywords", $"{playerNames}, {championNames}", Field.Store.YES));

        _writer.AddDocument(document);
    }

    public IEnumerable<string> Query(string searchTerm, int maxEntries, float minScore = 0.4f)
    {
        var query = _queryParser.Parse(searchTerm);
        var topDocs = _searcher.Search(query, n: maxEntries);

        // return ids that pass the minimum score
        return topDocs.ScoreDocs.Where(x => x.Score > minScore).Select(x => _searcher.Doc(x.Doc).Get("id"));
    }
}
