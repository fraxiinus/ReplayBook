using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Rofl.Requests.Models;
using Rofl.Logger;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Rofl.Settings.Models;

namespace Rofl.Requests.Utilities
{
    public class DownloadClient
    {
        private readonly string _downloadRootFolder;
        private readonly ObservableSettings _settings;
        private readonly Scribe _log;
        private readonly string _myName;
        private readonly HttpClient _httpClient;

        private const string UserAgent = @"ReplayBook/PR5 (+https://github.com/leeanchu/ReplayBook)";

        public DownloadClient(string downloadPath, ObservableSettings settings, Scribe log)
        {
            if (string.IsNullOrEmpty(downloadPath))
            {
                throw new ArgumentNullException(nameof(downloadPath));
            }

            _settings = settings;
            _log = log;
            _downloadRootFolder = downloadPath;
            _myName = this.GetType().ToString();
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
                    RequestUrl = downloadUrl,
                    ResponseDate = DateTime.Now,
                    ResponsePath = null
                };
            }

            var filePath = await DownloadImage(downloadUrl, downloadLocation).ConfigureAwait(false);
            // failed to download an image!
            if(filePath == null)
            {
                return new ResponseBase()
                {
                    DataVersion = request.DataDragonVersion,
                    Request = request,
                    IsFaulted = true,
                    RequestUrl = downloadUrl,
                    ResponseDate = DateTime.Now,
                    ResponsePath = null
                };
            }

            return new ResponseBase()
            {
                DataVersion = request.DataDragonVersion,
                Request = request,
                IsFaulted = false,
                RequestUrl = downloadUrl,
                ResponseDate = DateTime.Now,
                ResponsePath = filePath
            };
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
                    _log.Error(_myName, $"Unable to send HTTP request to {url}");
                    return null;
                }
            }

            if (response.IsSuccessStatusCode)
            {
                _log.Information(_myName, $"Made successful HTTP request {url}");

                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

                return JArray.Parse(json).ToObject<string[]>();
            }
            else
            {
                _log.Error(_myName, $"HTTP request failed {(int)response.StatusCode} {url}");
                return null;
            }
        }

        // TODO this feature to be used in first time tutorial/configuration screen
        public async Task<IEnumerable<string>> GetAllChampions()
        {
            var version = (await GetDataDragonVersionStringsAsync().ConfigureAwait(true)).FirstOrDefault();
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
                    _log.Error(_myName, $"Unable to send HTTP request to {url}");
                    return null;
                }
            }

            // Load response into JObject
            if (response.IsSuccessStatusCode)
            {
                _log.Information(_myName, $"Made successful HTTP request {url}");

                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

                responseObject = JObject.Parse(json);
            }
            else
            {
                _log.Error(_myName, $"HTTP request failed {(int)response.StatusCode} {url}");
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

        private async Task<string> DownloadImage(string url, string location)
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
                    _log.Error(_myName, $"Unable to send HTTP request to {url}");
                    return null;
                }
            }

            if (response.IsSuccessStatusCode)
            {
                _log.Information(_myName, $"Made successful HTTP request {url}");
                using (var s = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    // Creates or overwrites the file
                    if (!Directory.Exists(Path.GetDirectoryName(location)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(location));
                    }
                    using (var file = File.Create(location))
                    {
                        await s.CopyToAsync(file).ConfigureAwait(true);
                    }
                }
                return location;
            }
            else
            {
                _log.Warning(_myName, $"HTTP request failed {(int) response.StatusCode} {url}");
                return null;
            }
        }
    }
}
