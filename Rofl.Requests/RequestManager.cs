using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Rofl.Requests.Utilities;
using Rofl.Requests.Models;
using System.IO;

namespace Rofl.Requests
{
    public class RequestManager
    {
        private readonly DownloadClient _downloadClient = new DownloadClient(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache"));

        private readonly CacheClient _cacheClient = new CacheClient(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache"));

        private readonly string _exceptionOriginName = "Rofl.Requests.RequestManager";

        public string DataDragonVersion { get; private set; } = null;

        public async Task<ResponseBase> MakeRequestAsync(RequestBase request)
        {
            if(string.IsNullOrEmpty(DataDragonVersion))
            {
                throw new Exception("Data dragon version must be set before making requests");
            }

            ResponseBase cacheResponse;
            // Check cache first, return response if found
            try
            {
                cacheResponse = _cacheClient.CheckImageCache(request);
            }
            catch (OutOfMemoryException ex)
            {
                return new ResponseBase()
                {
                    Exception = ex,
                    IsFaulted = true
                };
            }

            if (!cacheResponse.IsFaulted)
            {
                return cacheResponse;
            }

            // Does not exist in cache, make download request
            try
            {
                return await _downloadClient.DownloadIconImageAsync(request, DataDragonVersion);
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
        /// Given replay version string, set appropriate DataDragon version.
        /// Only compares first two numbers.
        /// </summary>
        public async Task SetDataDragonVersionAsync(string replayVersion)
        {
            string versionRef = replayVersion.VersionSubstring();
            if(string.IsNullOrEmpty(versionRef))
            {
                throw new ArgumentException($"{_exceptionOriginName} - Replay version: \"{replayVersion}\" is not valid.");
            }

            string[] allVersions;

            // Get all data dragon versions
            try
            {
                allVersions = await GetDataDragonVersionStringsAsync();

            } catch (Exception ex)
            {
                throw new Exception($"{_exceptionOriginName} - Error requesting Data Dragon versions\n\n{ex.GetType().ToString()} - {ex.Message}");
            }

            string versionQueryResult = (from version in allVersions
                                         where version.StartsWith(versionRef)
                                         select version).FirstOrDefault();

            // TODO to add caching,if query doesn't have any results, call GetDataDragonVersions again
            // If it still returns no results, return default (maybe error?)
            DataDragonVersion = string.IsNullOrEmpty(versionQueryResult) ? allVersions.First() : versionQueryResult;
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
