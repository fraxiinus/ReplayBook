using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Rofl.Requests.Utilities;
using Rofl.Requests.Models;
using System.IO;
using Microsoft.Extensions.Configuration;
using Rofl.Logger;

namespace Rofl.Requests
{
    public class RequestManager
    {
        private readonly DownloadClient _downloadClient;

        private readonly CacheClient _cacheClient;

        private readonly string _myName;

        private Scribe _log;
        private IConfiguration _config;
        private string _cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");

        public RequestManager(IConfiguration config, Scribe log)
        {
            _config = config;
            _log = log;

            // TODO these should use log and config
            _downloadClient = new DownloadClient(_cachePath, _config, _log);
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
                return await _downloadClient.DownloadIconImageAsync(request);
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
            string versionRef = replayVersion.VersionSubstring();
            if(string.IsNullOrEmpty(versionRef))
            {
                string errorMsg = $"{_myName} - Replay version: \"{replayVersion}\" is not valid";
                _log.Error(_myName, errorMsg);
                throw new ArgumentException(errorMsg);
            }

            string[] allVersions;

            // Get all data dragon versions
            try
            {
                allVersions = await GetDataDragonVersionStringsAsync();

            } catch (Exception ex)
            {
                string errorMsg = $"{_myName} - Error requesting Data Dragon versions\n\n{ex.GetType().ToString()} - {ex.Message}";
                _log.Error(_myName, errorMsg);
                throw new Exception(errorMsg);
            }

            string versionQueryResult = (from version in allVersions
                                         where version.StartsWith(versionRef)
                                         select version).FirstOrDefault();

            // TODO to add caching,if query doesn't have any results, call GetDataDragonVersions again
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
                string result = await wc.DownloadStringTaskAsync(@"https://ddragon.leagueoflegends.com/api/versions.json");
                return JArray.Parse(result).ToObject<string[]>();
            }
        }
    }
}
