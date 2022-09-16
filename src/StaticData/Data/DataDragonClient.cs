using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.StaticData.Models;
using System.IO;
using System.Net.Http;
using System.Text.Json;

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
        public async Task<IEnumerable<string>> GetPatchesAsync(CancellationToken cancellationToken = default)
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
                    response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(true);
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

                var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(true);

                return JsonSerializer.Deserialize<string[]>(json)
                    ?? throw new Exception("Json deserialized to null value");
            }
            else
            {
                throw new HttpRequestException($"HTTP request to Data Dragon failed on {(int)response.StatusCode}, {url}", null, response.StatusCode);
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
        public async Task<IEnumerable<string>> DownloadSpriteImages(string patchVersion, string dataType, CancellationToken cancellationToken = default)
        {
            if (dataType == StaticDataDefinitions.Rune)
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
                    + dataType.ToLower()
                    + resultFiles.Count
                    + ".png";

                var response = await SendGetRequestAsync(url, "image/*", cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    _log.Information($"Made successful HTTP request {url}");

                    var destinationFile = await SaveImageToFile(await response.Content.ReadAsStreamAsync(cancellationToken), resultFiles.Count, patchVersion, dataType, cancellationToken);

                    resultFiles.Add(destinationFile);
                }
                else
                {
                    // if the very first request fails, then we might have an internet problem
                    if (resultFiles.Count == 0)
                    {
                        throw new HttpRequestException($"HTTP request to Data Dragon failed on {(int)response.StatusCode}, {url}", null, response.StatusCode);
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

        public async Task<IEnumerable<(string key, string relativePath)>> DownloadRuneImages(string patchVersion, IEnumerable<RuneProperties> runeData, CancellationToken cancellationToken = default)
        {
            var resultFiles = new List<(string, string)>();

            foreach (var rune in runeData)
            {
                if (rune.Key == null)
                {
                    _log.Information("encountered null rune");
                    continue;
                }

                var url = _config.DataDragonBaseUrl
                    + "img/"
                    + rune.IconUrl;

                var response = await SendGetRequestAsync(url, "image/*", cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    _log.Information($"Made successful HTTP request {url}");

                    var destinationFile = await SaveRuneImageToFile(await response.Content.ReadAsStreamAsync(cancellationToken), patchVersion, rune.Key + ".png", cancellationToken);
                    
                    resultFiles.Add((rune.Key, destinationFile));
                }
            }

            return resultFiles;
        }

        /// <summary>
        /// Downloads Data Dragon properties for a given patch, type, and language
        /// </summary>
        /// <param name="patchVersion"></param>
        /// <param name="dataType"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<IEnumerable<BaseStaticProperties>> DownloadPropertySet(string patchVersion, string dataType, string language, CancellationToken cancellationToken = default)
        {
            // catch and rename rune data type
            var fileName = dataType == StaticDataDefinitions.Rune ? "runesReforged" : dataType.ToLower();

            var url = _config.DataDragonBaseUrl
                + patchVersion
                + "/data/"
                + language
                + "/"
                + fileName
                + ".json";

            var response = await SendGetRequestAsync(url, "application/json", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                if (dataType == StaticDataDefinitions.Rune)
                {
                    _log.Information($"Made successful HTTP request {url}");

                    return await ParseRunePropertySet(response.Content.ReadAsStream(cancellationToken), cancellationToken);
                }
                else
                {
                    return await ParsePropertySet(response.Content.ReadAsStream(cancellationToken), dataType, cancellationToken);
                }
            }
            else
            {
                throw new HttpRequestException($"HTTP request to Data Dragon failed on {(int)response.StatusCode}, {url}", null, response.StatusCode);
            }
        }

        private async Task<IEnumerable<BaseStaticProperties>> ParsePropertySet(Stream jsonStream, string dataType, CancellationToken cancellationToken = default)
        {
            if (dataType == StaticDataDefinitions.Rune)
            {
                throw new NotSupportedException("Runes need to be parsed in their special function");
            }

            var results = new List<BaseStaticProperties>();

            using var document = await JsonDocument.ParseAsync(jsonStream, cancellationToken: cancellationToken);
            var dataArray = document.RootElement.GetProperty("data");
            foreach (var element in dataArray.EnumerateObject())
            {
                switch (dataType)
                {
                    case StaticDataDefinitions.Champion:
                        results.Add(CreateStaticData(element, dataType));
                        break;
                    case StaticDataDefinitions.Item:
                        results.Add(CreateStaticData(element, dataType));
                        break;
                }
            }

            return results;
        }

        private async Task<IEnumerable<RuneProperties>> ParseRunePropertySet(Stream jsonStream, CancellationToken cancellationToken = default)
        {
            var results = new List<RuneProperties>();

            using var document = await JsonDocument.ParseAsync(jsonStream, cancellationToken: cancellationToken);
            foreach (var tree in document.RootElement.EnumerateArray())
            {
                foreach (var slot in tree.GetProperty("slots").EnumerateArray())
                {
                    foreach (var rune in slot.GetProperty("runes").EnumerateArray())
                    {
                        results.Add(CreateRuneData(rune));
                    }
                }
            }

            return results;
        }

        private BaseStaticProperties CreateStaticData(JsonProperty jsonProperty, string dataType)
        {
            if (dataType == StaticDataDefinitions.Rune)
            {
                throw new NotSupportedException("Runes need to be created in their special function");
            }

            // item and champion data have the same structure
            var id = jsonProperty.Name;

            BaseStaticProperties instance = dataType switch
            {
                StaticDataDefinitions.Champion => new ChampionProperties(id),
                StaticDataDefinitions.Item => new ItemProperties(id),
                _ => throw new Exception($"Invalid data type sent {dataType}")
            };
            
            instance.DisplayName = jsonProperty.Value.GetProperty("name").GetString()
                ?? throw new Exception("json name returned null");

            var image = jsonProperty.Value.GetProperty("image");
            instance.ImageProperties = new ImageProperties()
            {
                Source = image.GetProperty("sprite").ToString(), 
                X = image.GetProperty("x").GetInt32(),
                Y = image.GetProperty("y").GetInt32(),
                Height = image.GetProperty("h").GetInt32(),
                Width = image.GetProperty("w").GetInt32()
            };

            return instance;
        }

        private RuneProperties CreateRuneData(JsonElement jsonElement)
        {
            var id = jsonElement.GetProperty("id").GetInt32().ToString()
                ?? throw new Exception("Element ID parsed null");

            var instance = new RuneProperties(id)
            {
                DisplayName = jsonElement.GetProperty("name").GetString()
                    ?? throw new Exception("json name returned null"),
                Key = jsonElement.GetProperty("key").GetString(),
                IconUrl = jsonElement.GetProperty("icon").GetString()
            };

            return instance;
        }

        private async Task<HttpResponseMessage> SendGetRequestAsync(string url, string accept, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response;
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            // add headers so server can identify our queries (to be polite)
            request.Headers.UserAgent.ParseAdd(_userAgent);
            request.Headers.Accept.ParseAdd(accept);
            try
            {
                response = await _httpClient.SendAsync(request, cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                _log.Error($"Unable to send HTTP request to {url}, Exception: {ex}");
                throw;
            }

            return response;
        }

        private async Task<string> SaveImageToFile(Stream imageStream, int count, string patchVersion, string dataType, CancellationToken cancellationToken = default)
        {
            // Make sure destination exists
            var relativeDestination = Path.Combine(patchVersion, dataType.ToLower());
            var destinationFolder = Path.Combine(_dataPath, relativeDestination);
            Directory.CreateDirectory(destinationFolder);

            // Create file stream that will write file
            var destinationFile = Path.Combine(destinationFolder, $"{count}.png");
            using var fileStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write);
            await imageStream.CopyToAsync(fileStream, cancellationToken);

            return Path.Combine(relativeDestination, $"{count}.png");
        }

        private async Task<string> SaveRuneImageToFile(Stream imageStream, string relDestFolder, string destinationFileName, CancellationToken cancellationToken = default)
        {
            var destinationFolder = Path.Combine(_dataPath, relDestFolder, "rune");

            Directory.CreateDirectory(destinationFolder);

            var destinationFile = Path.Combine(destinationFolder, destinationFileName);
            using var filestream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write);
            await imageStream.CopyToAsync(filestream, cancellationToken);

            return Path.Combine(relDestFolder, "rune", destinationFileName);
        }
    }
}
