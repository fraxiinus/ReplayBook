using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.StaticData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fraxiinus.ReplayBook.StaticData.Data
{
    public class DataDragonClient
    {
        private readonly RiZhi _log;
        private readonly ObservableConfiguration _config;
        private readonly string _userAgent;
        private readonly string _dataPath;

        private readonly HttpClient _httpClient;

        public DataDragonClient(string dataPath, ObservableConfiguration config, string userAgent, RiZhi log)
        {
            _dataPath = dataPath;
            _config = config;
            _userAgent = userAgent;
            _log = log;

            _httpClient = new HttpClient();
        }

        // http client needs to be properly disposed
        ~DataDragonClient()
        {
            _httpClient.Dispose();
        }

        /// <summary>
        /// Get an array of all appropriate DataDragon versions
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetPatchesAsync()
        {
            const string url = @"https://ddragon.leagueoflegends.com/api/versions.json";

            HttpResponseMessage response;
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                // add headers so server can identify our queries (to be polite)
                request.Headers.UserAgent.ParseAdd(_userAgent);
                request.Headers.Accept.ParseAdd("text/json");

                try
                {
                    // send the request
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

                return JsonSerializer.Deserialize<string[]>(json)
                    ?? throw new Exception("Json deserialized to null value");
            }
            else
            {
                throw new HttpRequestException($"HTTP request to Data Dragon failed on {(int)response.StatusCode}, {url}");
            }
        }

        /// <summary>
        /// Downloads item or champion sprites for a given patch
        /// </summary>
        /// <param name="patchVersion"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<IEnumerable<string>> GetSpriteImages(string patchVersion, StaticDataType dataType)
        {
            if (dataType == StaticDataType.Rune)
            {
                throw new NotSupportedException("Runes cannot be downloaded as sprites");
            }

            var resultFiles = new List<string>();
            bool reachedFailure = false;
            while (!reachedFailure)
            {
                // construct request url
                var url = _config.DataDragonBaseUrl
                    + patchVersion
                    + "/img/sprite/"
                    + dataType.ToString().ToLower()
                    + resultFiles.Count
                    + ".png";

                HttpResponseMessage response;
                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    // add headers so server can identify our queries (to be polite)
                    request.Headers.UserAgent.ParseAdd(_userAgent);
                    request.Headers.Accept.ParseAdd("image/*");

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

                    var destinationFile = await SaveImageToFile(await response.Content.ReadAsStreamAsync(), resultFiles.Count, patchVersion, dataType);

                    resultFiles.Add(destinationFile);
                }
                else
                {
                    // if the very first request fails, then we might have an internet problem
                    if (resultFiles.Count == 0)
                    {
                        throw new HttpRequestException($"HTTP request to Data Dragon failed on {(int)response.StatusCode}, {url}");
                    }
                    else // otherwise it marks the end of the sprites (very ugly implementation but I can't think of any better ahaha)
                    {
                        _log.Information($"Sprite request failure at {dataType}-{resultFiles.Count}");
                        reachedFailure = true;
                    }
                }
            }

            return resultFiles;
        }

        private async Task<string> SaveImageToFile(Stream imageStream, int count, string patchVersion, StaticDataType dataType)
        {
            // Make sure destination exists
            var destinationFolder = Path.Combine(_dataPath, patchVersion, dataType.ToString().ToLower());
            Directory.CreateDirectory(destinationFolder);

            // Create file stream that will write file
            var destinationFile = Path.Combine(destinationFolder, $"{count}.png");
            using var fileStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write);
            await imageStream.CopyToAsync(fileStream);

            return destinationFile;
        }
    }
}
