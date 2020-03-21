using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Rofl.Requests.Utilities;
using Rofl.Requests.Models;
using System.IO;
using Rofl.Logger;
using Rofl.Settings.Models;

namespace Rofl.Requests
{
    public class RequestManager
    {
        private readonly DownloadClient _downloadClient;

        private readonly CacheClient _cacheClient;

        private readonly string _myName;

        private readonly Scribe _log;
        private readonly ObservableSettings _settings;
        private readonly string _cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");

        public RequestManager(ObservableSettings settings, Scribe log)
        {
            _settings = settings;
            _log = log;

            _myName = this.GetType().ToString();

            // TODO these should use log and config
            _downloadClient = new DownloadClient(_cachePath, _settings, _log);
            _cacheClient = new CacheClient(_cachePath, _log);
        }

        public async Task<ResponseBase> MakeRequestAsync(RequestBase request)
        {

            // Check cache first, return response if found
            ResponseBase cacheResponse = _cacheClient.CheckImageCache(request);

            // Fault occurs if cache is unable to find the file, or if the file is corrupted
            if (!cacheResponse.IsFaulted)
            {
                return cacheResponse;
            }

            // Does not exist in cache, make download request
            try
            {
                return await _downloadClient.DownloadIconImageAsync(request).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                return new ResponseBase()
                {
                    Exception = ex,
                    IsFaulted = true
                };
            }
        }

        /// <summary>
        /// Given replay version string, returns appropriate DataDragon version.
        /// Only compares first two numbers.
        /// </summary>
        public async Task<string> GetDataDragonVersionAsync(string replayVersion)
        {
            var allVersions = await GetDataDragonVersionStringsAsync().ConfigureAwait(true);

            // Return most recent patch number
            if (_settings.UseMostRecent)
            {
                return allVersions.FirstOrDefault();
            }

            var versionRef = replayVersion.VersionSubstring();
            if (string.IsNullOrEmpty(versionRef))
            {
                var errorMsg = $"{_myName} - Replay version: \"{replayVersion}\" is not valid";
                _log.Error(_myName, errorMsg);
                throw new ArgumentException(errorMsg);
            }

            var versionQueryResult = (from version in allVersions
                where version.StartsWith(versionRef, StringComparison.OrdinalIgnoreCase)
                select version).FirstOrDefault();

            // If it still returns no results, return default (maybe error?)
            return string.IsNullOrEmpty(versionQueryResult) ? allVersions.First() : versionQueryResult;
        }

        /// <summary>
        /// Get an array of all appropriate DataDragon versions
        /// </summary>
        /// <returns></returns>
        private async Task<string[]> GetDataDragonVersionStringsAsync()
        {
            // TODO Maybe make this cache?
            // So it saves the file somewhere, 
            // if the set method doesn't find any matches, make a new request
            using (WebClient wc = new WebClient())
            {
                string result = await wc.DownloadStringTaskAsync(@"https://ddragon.leagueoflegends.com/api/versions.json").ConfigureAwait(true);
                return JArray.Parse(result).ToObject<string[]>();
            }
        }
    }
}
