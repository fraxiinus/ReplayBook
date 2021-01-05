using Etirps.RiZhi;
using Newtonsoft.Json.Linq;
using Rofl.Requests.Models;
using Rofl.Settings.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rofl.Requests.Utilities
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
    public class DownloadClient
    {
        private readonly string _downloadRootFolder;
        private readonly ObservableSettings _settings;
        private readonly RiZhi _log;
        private readonly HttpClient _httpClient;

        private const string UserAgent = @"ReplayBook/PR9.3 (+https://github.com/fraxiinus/ReplayBook)";

        private string LatestDataDragonVersion = null;

        public DownloadClient(string downloadPath, ObservableSettings settings, RiZhi log)
        {
            if (string.IsNullOrEmpty(downloadPath))
            {
                throw new ArgumentNullException(nameof(downloadPath));
            }

            _settings = settings;
            _log = log;
            _downloadRootFolder = downloadPath;
            _httpClient = new HttpClient();
        }

        ~DownloadClient()
        {
            _httpClient.Dispose();
        }

        /// <summary>
        /// Processes image download requests
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseBase> DownloadIconImageAsync(RequestBase request)
        {
            if(request == null) { throw new ArgumentNullException(nameof(request)); }

            if (string.IsNullOrEmpty(request.DataDragonVersion))
            {
                throw new ArgumentNullException(nameof(request));
            }

            var downloadUrl = ConstructRequestUrl(request);
            var downloadLocation = ConstructDownloadPath(request);

            if (string.IsNullOrEmpty(downloadUrl))
            {
                return new ResponseBase()
                {
                    DataVersion = request.DataDragonVersion,
                    Request = request,
                    IsFaulted = false,
                    FromCache = false,
                    RequestUrl = downloadUrl,
                    ResponseDate = DateTime.Now,
                    ResponsePath = null,
                    ResponseBytes = null
                };
            }

            var bytes = await DownloadImage(downloadUrl, downloadLocation).ConfigureAwait(false);
            // failed to download an image!
            if(bytes == null)
            {
                return new ResponseBase()
                {
                    DataVersion = request.DataDragonVersion,
                    Request = request,
                    IsFaulted = true,
                    FromCache = false,
                    RequestUrl = downloadUrl,
                    ResponseDate = DateTime.Now,
                    ResponsePath = null,
                    ResponseBytes = null
                };
            }

            return new ResponseBase()
            {
                DataVersion = request.DataDragonVersion,
                Request = request,
                IsFaulted = false,
                FromCache = false,
                RequestUrl = downloadUrl,
                ResponseDate = DateTime.Now,
                ResponsePath = downloadLocation,
                ResponseBytes = bytes
            };
        }

        public async Task<string> GetLatestDataDragonVersion()
        {
            if (string.IsNullOrEmpty(LatestDataDragonVersion))
            {
                var latest = (await GetDataDragonVersionStringsAsync().ConfigureAwait(true))
                    .FirstOrDefault();
                LatestDataDragonVersion = latest;
            }

            return LatestDataDragonVersion;
        }


        /// <summary>
        /// Get an array of all appropriate DataDragon versions
        /// </summary>
        /// <returns></returns>
        public async Task<string[]> GetDataDragonVersionStringsAsync()
        {
            const string url = @"https://ddragon.leagueoflegends.com/api/versions.json";

            HttpResponseMessage response;
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.UserAgent.ParseAdd(UserAgent);
                request.Headers.Accept.ParseAdd("text/json");

                try
                {
                    response = await _httpClient.SendAsync(request).ConfigureAwait(true);
                }
                catch (HttpRequestException)
                {
                    _log.Error($"Unable to send HTTP request to {url}");
                    return null;
                }
            }

            if (response.IsSuccessStatusCode)
            {
                _log.Information($"Made successful HTTP request {url}");

                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

                return JArray.Parse(json).ToObject<string[]>();
            }
            else
            {
                _log.Error($"HTTP request failed {(int)response.StatusCode} {url}");
                return null;
            }
        }

        public async Task<IEnumerable<string>> GetAllChampionNames()
        {
            var version = await GetLatestDataDragonVersion().ConfigureAwait(true);
            var url = @"http://ddragon.leagueoflegends.com/cdn/" + version + @"/data/en_US/champion.json";

            JObject responseObject;

            // Make request to get all champions json
            HttpResponseMessage response;
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.UserAgent.ParseAdd(UserAgent);
                request.Headers.Accept.ParseAdd("text/json");

                try
                {
                    response = await _httpClient.SendAsync(request).ConfigureAwait(true);
                }
                catch (HttpRequestException)
                {
                    _log.Error($"Unable to send HTTP request to {url}");
                    return null;
                }
            }

            // Load response into JObject
            if (response.IsSuccessStatusCode)
            {
                _log.Information($"Made successful HTTP request {url}");

                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

                responseObject = JObject.Parse(json);
            }
            else
            {
                _log.Error($"HTTP request failed {(int)response.StatusCode} {url}");
                return null;
            }

            return (from JProperty champion in responseObject["data"] select champion.Name).ToList();
        }

        public async Task<IEnumerable<string>> GetAllItemNumbers()
        {
            var version = await GetLatestDataDragonVersion().ConfigureAwait(true);
            var url = @"http://ddragon.leagueoflegends.com/cdn/" + version + @"/data/en_US/item.json";

            JObject responseObject;

            // Make request to get all champions json
            HttpResponseMessage response;
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.UserAgent.ParseAdd(UserAgent);
                request.Headers.Accept.ParseAdd("text/json");

                try
                {
                    response = await _httpClient.SendAsync(request).ConfigureAwait(true);
                }
                catch (HttpRequestException)
                {
                    _log.Error($"Unable to send HTTP request to {url}");
                    return null;
                }
            }

            // Load response into JObject
            if (response.IsSuccessStatusCode)
            {
                _log.Information($"Made successful HTTP request {url}");

                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

                responseObject = JObject.Parse(json);
            }
            else
            {
                _log.Error($"HTTP request failed {(int)response.StatusCode} {url}");
                return null;
            }

            return (from JProperty champion in responseObject["data"] select champion.Name).ToList();
        }

        private string ConstructRequestUrl(RequestBase request)
        {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }

            if (string.IsNullOrEmpty(request.DataDragonVersion))
            {
                throw new ArgumentNullException(nameof(request));
            }

            string downloadUrl = null;
            switch (request)
            {
                case ChampionRequest c:
                {
                    if (c.ChampionName == "FiddleSticks")
                    {
                        c.ChampionName = "Fiddlesticks";
                    }

                    downloadUrl = _settings.DataDragonBaseUrl + c.DataDragonVersion + _settings.ChampionRelativeUrl + c.ChampionName + ".png";
                    break;
                }
                case ItemRequest i:
                {
                    if (!i.ItemID.Equals("0", StringComparison.OrdinalIgnoreCase))
                    {
                        downloadUrl = _settings.DataDragonBaseUrl + i.DataDragonVersion + _settings.ItemRelativeUrl + i.ItemID + ".png";
                    }

                    break;
                }
                case MapRequest m:
                {
                    downloadUrl = _settings.DataDragonBaseUrl + m.DataDragonVersion + _settings.MapRelativeUrl + m.MapID + ".png";
                    break;
                }
            }

            return downloadUrl;
        }

        private string ConstructDownloadPath(RequestBase request)
        {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }

            if (string.IsNullOrEmpty(request.DataDragonVersion))
            {
                throw new ArgumentNullException(nameof(request));
            }

            string downloadPath = null;
            switch (request)
            {
                case ChampionRequest c:
                {
                    if (c.ChampionName == "FiddleSticks")
                    {
                        c.ChampionName = "Fiddlesticks";
                    }

                    downloadPath = Path.Combine(_downloadRootFolder, "champs", $"{c.ChampionName}.png");
                        break;
                }
                case ItemRequest i:
                {
                    if (!i.ItemID.Equals("0", StringComparison.OrdinalIgnoreCase))
                    {
                        downloadPath = Path.Combine(_downloadRootFolder, "items", $"{i.ItemID}.png");
                    }

                    break;
                }
                case MapRequest m:
                {
                    downloadPath = Path.Combine(_downloadRootFolder, "maps", $"{m.MapID}.png");
                    break;
                }
            }

            return downloadPath;
        }

        /// <summary>
        /// Downloads image from url and returns bitmap
        /// </summary>
        /// <param name="url"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private async Task<byte[]> DownloadImage(string url, string location)
        {
            HttpResponseMessage response;
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.UserAgent.ParseAdd(UserAgent);
                request.Headers.Accept.ParseAdd("image/png");

                try
                {
                    response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                }
                catch (HttpRequestException)
                {
                    _log.Error($"Unable to send HTTP request to {url}");
                    return null;
                }
            }

            if (response.IsSuccessStatusCode)
            {
                // _log.Information($"Made successful HTTP request {url}");
                var contentBytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

                // Create file directory if it does not exist
                if (!Directory.Exists(Path.GetDirectoryName(location)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(location));
                }
                // Write bytes to file
                File.WriteAllBytes(location, contentBytes);

                return contentBytes;
            }
            else
            {
                _log.Warning($"HTTP request failed {(int) response.StatusCode} {url}");
                return null;
            }
        }
    }
}
