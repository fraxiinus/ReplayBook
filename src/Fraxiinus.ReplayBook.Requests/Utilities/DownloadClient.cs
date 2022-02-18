﻿using Etirps.RiZhi;
using Newtonsoft.Json.Linq;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.Requests.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fraxiinus.ReplayBook.Requests.Utilities
{
    public class DownloadClient
    {
        private readonly string _downloadRootFolder;
        private readonly ObservableConfiguration _config;
        private readonly RiZhi _log;
        private readonly HttpClient _httpClient;
        private readonly string _userAgent;

        private string LatestDataDragonVersion;

        public DownloadClient(string downloadPath, string userAgent, ObservableConfiguration config, RiZhi log)
        {
            if (string.IsNullOrEmpty(downloadPath))
            {
                throw new ArgumentNullException(nameof(downloadPath));
            }

            _config = config;
            _log = log;
            _downloadRootFolder = downloadPath;
            _userAgent = userAgent;
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
            if (request == null) { throw new ArgumentNullException(nameof(request)); }

            if (string.IsNullOrEmpty(request.DataDragonVersion)) { throw new ArgumentNullException(nameof(request)); }

            string downloadUrl = ConstructRequestUrl(request);
            string downloadLocation = ConstructDownloadPath(request);

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

            byte[] bytes;
            try
            {
                bytes = await DownloadImage(downloadUrl, downloadLocation).ConfigureAwait(false);
            }
            catch (HttpRequestException)
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

            // failed to download an image!
            if (bytes == null)
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
            // check if we have a value cached already
            if (string.IsNullOrEmpty(LatestDataDragonVersion))
            {
                string[] versionList = await GetDataDragonVersionStringsAsync().ConfigureAwait(true);

                // Cache the value and return
                LatestDataDragonVersion = versionList.FirstOrDefault();
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
                request.Headers.UserAgent.ParseAdd(_userAgent);
                request.Headers.Accept.ParseAdd("text/json");

                try
                {
                    response = await _httpClient.SendAsync(request).ConfigureAwait(true);
                }
                catch (HttpRequestException ex)
                {
                    _log.Error($"Unable to send HTTP request to {url}, Exception: {ex}");
                    throw;
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
                throw new HttpRequestException($"HTTP request to Data Dragon failed on {(int)response.StatusCode}, {url}");
            }
        }

        public async Task<IEnumerable<string>> GetAllChampionNames()
        {
            var version = await GetLatestDataDragonVersion().ConfigureAwait(true);
            var url = _config.DataDragonBaseUrl + version + @"/data/en_US/champion.json";

            JObject responseObject;

            // Make request to get all champions json
            HttpResponseMessage response;
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.UserAgent.ParseAdd(_userAgent);
                request.Headers.Accept.ParseAdd("text/json");

                try
                {
                    response = await _httpClient.SendAsync(request).ConfigureAwait(true);
                }
                catch (HttpRequestException ex)
                {
                    _log.Error($"Unable to send HTTP request to {url}, Exception: {ex}");
                    throw;
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
                throw new HttpRequestException($"HTTP request to Data Dragon failed on {(int)response.StatusCode}, {url}");
            }

            return (from JProperty champion in responseObject["data"] select champion.Name).ToList();
        }

        public async Task<IEnumerable<string>> GetAllItemNumbers()
        {
            var version = await GetLatestDataDragonVersion().ConfigureAwait(true);
            var url = _config.DataDragonBaseUrl + version + @"/data/en_US/item.json";

            JObject responseObject;

            // Make request to get all champions json
            HttpResponseMessage response;
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.UserAgent.ParseAdd(_userAgent);
                request.Headers.Accept.ParseAdd("text/json");

                try
                {
                    response = await _httpClient.SendAsync(request).ConfigureAwait(true);
                }
                catch (HttpRequestException ex)
                {
                    _log.Error($"Unable to send HTTP request to {url}, Exception: {ex}");
                    throw;
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
                throw new HttpRequestException($"HTTP request to Data Dragon failed on {(int)response.StatusCode}, {url}");
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
                    // Fix for fiddlesticks, name is different from replays
                    if (c.ChampionName == "FiddleSticks")
                    {
                        c.ChampionName = "Fiddlesticks";
                    }

                    downloadUrl = _config.DataDragonBaseUrl + c.DataDragonVersion + "/img/champion/" + c.ChampionName + ".png";
                    break;
                case ItemRequest i:
                    // make sure we aren't downloading item 0
                    if (!i.ItemID.Equals("0", StringComparison.OrdinalIgnoreCase))
                    {
                        downloadUrl = _config.DataDragonBaseUrl + i.DataDragonVersion + "/img/item/" + i.ItemID + ".png";
                    }

                    break;
                case MapRequest m:
                    downloadUrl = _config.DataDragonBaseUrl + m.DataDragonVersion + "/img/map/map" + m.MapID + ".png";
                    break;
                case RuneRequest r:
                    downloadUrl = _config.DataDragonBaseUrl + "img/" + r.TargetPath;
                    break;
                default:
                    throw new NotSupportedException($"unsupported request type: {request.GetType()}");
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
                    if (c.ChampionName == "FiddleSticks")
                    {
                        c.ChampionName = "Fiddlesticks";
                    }

                    downloadPath = Path.Combine(_downloadRootFolder, "champs", $"{c.ChampionName}.png");
                    break;
                case ItemRequest i:
                    if (!i.ItemID.Equals("0", StringComparison.OrdinalIgnoreCase))
                    {
                        downloadPath = Path.Combine(_downloadRootFolder, "items", $"{i.ItemID}.png");
                    }

                    break;
                case MapRequest m:
                    downloadPath = Path.Combine(_downloadRootFolder, "maps", $"{m.MapID}.png");
                    break;
                case RuneRequest r:
                    downloadPath = Path.Combine(_downloadRootFolder, "runes", $"{r.RuneKey}.png");
                    break;
                default:
                    throw new NotSupportedException($"unsupported request type: {request.GetType()}");
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
                request.Headers.UserAgent.ParseAdd(_userAgent);
                request.Headers.Accept.ParseAdd("image/png");

                try
                {
                    response = await _httpClient.SendAsync(request).ConfigureAwait(false);
                }
                catch (HttpRequestException ex)
                {
                    _log.Error($"Unable to send HTTP request to {url}, Exception: {ex}");
                    throw;
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
                string errorMsg = $"HTTP request to Data Dragon failed on {(int)response.StatusCode}, {url}";
                _log.Error(errorMsg);
                throw new HttpRequestException(errorMsg);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
