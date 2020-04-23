﻿using Etirps.RiZhi;
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
    public class RequestManager
    {
        private readonly DownloadClient _downloadClient;

        private readonly CacheClient _cacheClient;

        private readonly RiZhi _log;
        private readonly ObservableSettings _settings;
        private readonly string _cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");

        private readonly ConcurrentDictionary<string, Task<ResponseBase>> _inProgressTasks;
        //private readonly List<string> _inProgressIndex;

        public RequestManager(ObservableSettings settings, RiZhi log)
        {
            _settings = settings;
            _log = log ?? throw new ArgumentNullException(nameof(log));

            _downloadClient = new DownloadClient(_cachePath, _settings, _log);
            _cacheClient = new CacheClient(_cachePath, _log);

            _inProgressTasks = new ConcurrentDictionary<string, Task<ResponseBase>>();
            //_inProgressIndex = new List<string>();
        }

        public async Task<ResponseBase> MakeRequestAsync(RequestBase request)
        {
            // This acts as the key to tell if a download is in progress
            var requestId = GetRequestIdentifier(request);

            _log.Information($"Making request to {requestId}");

            // If a download is in progress, use the same task to get the result
            if(_inProgressTasks.ContainsKey(requestId))
            {
                // I hope this doesn't download twice
                var responseTask = _inProgressTasks[requestId];

                _log.Information($"Found existing task for {requestId}");

                if (responseTask.IsCompleted)
                {
                    _log.Information($"Task is completed, remove {requestId}");
                    if (!_inProgressTasks.TryRemove(requestId, out _))
                    {
                        _log.Warning($"Failed to remove in progress task {requestId}");
                    }
                }

                var result = await responseTask.ConfigureAwait(true);
                _log.Information($"{requestId} task finished and returned {!result.IsFaulted}");
                return result;
            }

            // A download is not in progress, is it cached?
            var cacheResponse = _cacheClient.CheckImageCache(request);

            // Fault occurs if cache is unable to find the file, or if the file is corrupted
            if (!cacheResponse.IsFaulted)
            {
                _log.Information($"Found {requestId} in cache");
                return cacheResponse;
            }

            // Does not exist in cache, make download request
            try
            {
                _log.Information($"Downloading {requestId}");
                var responseTask = _downloadClient.DownloadIconImageAsync(request);
                if (!_inProgressTasks.TryAdd(requestId, responseTask))
                {
                    _log.Warning($"Failed to add in progress task {requestId}");
                }

                var result = await responseTask.ConfigureAwait(true);
                _log.Information($"Completed download for {requestId}, returned {!result.IsFaulted}");
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
        public async Task<string> GetDataDragonVersionAsync(string replayVersion)
        {
            var allVersions = await _downloadClient.GetDataDragonVersionStringsAsync().ConfigureAwait(true);

            // Return most recent patch number
            if (_settings.UseMostRecent || string.IsNullOrEmpty(replayVersion))
            {
                return allVersions.FirstOrDefault();
            }

            var versionRef = replayVersion.VersionSubstring();
            if (string.IsNullOrEmpty(versionRef))
            {
                var errorMsg = $"Replay version: \"{replayVersion}\" is not valid";
                _log.Error(errorMsg);
                throw new ArgumentException(errorMsg);
            }

            var versionQueryResult = (from version in allVersions
                where version.StartsWith(versionRef, StringComparison.OrdinalIgnoreCase)
                select version).FirstOrDefault();

            // If it still returns no results, return default (maybe error?)
            return string.IsNullOrEmpty(versionQueryResult) ? allVersions.First() : versionQueryResult;
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
