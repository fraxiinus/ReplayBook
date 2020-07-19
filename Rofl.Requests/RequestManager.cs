using Etirps.RiZhi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rofl.Requests.Utilities;
using Rofl.Requests.Models;
using System.IO;
using Rofl.Settings.Models;

namespace Rofl.Requests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    public class RequestManager
    {
        private readonly DownloadClient _downloadClient;

        private readonly CacheClient _cacheClient;

        private readonly RiZhi _log;
        private readonly ObservableSettings _settings;

        // The cache directory
        private readonly string _cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");

        // Used to keep track of current tasks
        private readonly ConcurrentDictionary<string, Task<ResponseBase>> _inProgressTasks;

        // Prevent unnecessary calls for data dragon version
        private string _latestDataDragonVersion = null;

        public RequestManager(ObservableSettings settings, RiZhi log)
        {
            _settings = settings;
            _log = log ?? throw new ArgumentNullException(nameof(log));

            _downloadClient = new DownloadClient(_cachePath, _settings, _log);
            _cacheClient = new CacheClient(_cachePath, _log);

            _inProgressTasks = new ConcurrentDictionary<string, Task<ResponseBase>>();
        }

        public async Task<ResponseBase> MakeRequestAsync(RequestBase request)
        {
            // This acts as the key to tell if a download is in progress
            var requestId = GetRequestIdentifier(request);

            if (requestId == "0")
            {
                _log.Warning($"Invalid requestId: {requestId}");
                return new ResponseBase()
                {
                    Exception = new Exception($"requestId is not valid: {requestId}"),
                    IsFaulted = true
                };
            }

            // If a download is in progress, use the same task to get the result
            if(_inProgressTasks.ContainsKey(requestId))
            {
                // Get the matching in progress task
                var responseTask = _inProgressTasks[requestId];

                // If the task is complete, remove it
                if (responseTask.IsCompleted)
                {
                    if (!_inProgressTasks.TryRemove(requestId, out _))
                    {
                        _log.Warning($"Failed to remove in progress task {requestId}");
                    }
                }

                // Get the result of the task and return it
                var result = await responseTask.ConfigureAwait(true);
                return result;
            }

            // A download is not in progress, is it cached?
            var cacheResponse = _cacheClient.CheckImageCache(request);

            // Fault occurs if cache is unable to find the file, or if the file is corrupted
            if (!cacheResponse.IsFaulted)
            {
                return cacheResponse;
            }

            // Does not exist in cache, make download request
            try
            {
                var responseTask = _downloadClient.DownloadIconImageAsync(request);
                if (!_inProgressTasks.TryAdd(requestId, responseTask))
                {
                    _log.Warning($"Failed to add in progress task {requestId}");
                }

                var result = await responseTask.ConfigureAwait(true);
                return result;
            }
            catch (Exception ex)
            {
                _log.Error($"Failed to download {requestId}. Ex: {ex}");
                return new ResponseBase()
                {
                    Exception = ex,
                    IsFaulted = true
                };
            }
        }

        public async Task<IEnumerable<ResponseBase>> MakeRequestsAsync(IEnumerable<RequestBase> requests)
        {
            if(requests == null) { throw new ArgumentNullException(nameof(requests)); }

            var results = new List<ResponseBase>();

            foreach (var request in requests)
            {
                results.Add(await MakeRequestAsync(request).ConfigureAwait(true));
            }

            return results;
        }

        /// <summary>
        /// Given replay version string, returns appropriate DataDragon version.
        /// Only compares first two numbers.
        /// </summary>
        public async Task<string> GetLatestDataDragonVersionAsync()
        {
            // If we have a saved data dragon version, return that instead
            if (!String.IsNullOrEmpty(_latestDataDragonVersion) && 
                !String.IsNullOrEmpty(_latestDataDragonVersion.VersionSubstring()))
            {
                return _latestDataDragonVersion;
            }

            var allVersions = await _downloadClient.GetDataDragonVersionStringsAsync().ConfigureAwait(true);

            _latestDataDragonVersion = allVersions.FirstOrDefault();

            return _latestDataDragonVersion;
        }

        public async Task<IEnumerable<ChampionRequest>> GetAllChampionRequests()
        {
            var championNames = await _downloadClient.GetAllChampionNames().ConfigureAwait(true);
            var latestVersion = await _downloadClient.GetLatestDataDragonVersion().ConfigureAwait(true);

            return championNames.Select(x => new ChampionRequest
            {
                ChampionName = x,
                DataDragonVersion = latestVersion
            });
        }

        public async Task<IEnumerable<ItemRequest>> GetAllItemRequests()
        {
            var itemNumbers = await _downloadClient.GetAllItemNumbers().ConfigureAwait(true);
            var latestVersion = await _downloadClient.GetLatestDataDragonVersion().ConfigureAwait(true);

            return itemNumbers.Select(x => new ItemRequest
            {
                ItemID = x,
                DataDragonVersion = latestVersion
            });
        }

        private string GetRequestIdentifier(RequestBase request)
        {
            string result = null;
            switch (request)
            {
                case ChampionRequest championRequest:
                    result = championRequest.ChampionName;
                    break;
                
                case MapRequest mapRequest:
                    result = mapRequest.MapID;
                    break;

                case ItemRequest itemRequest:
                    result = itemRequest.ItemID;
                    break;
            }

            return result;
        }
    }
}
