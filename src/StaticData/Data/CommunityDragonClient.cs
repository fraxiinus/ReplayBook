using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.StaticData.Models;
using System.Net.Http;
using System.Text.Json;

namespace Fraxiinus.ReplayBook.StaticData.Data
{
    public class CommunityDragonClient
    {
        private readonly RiZhi _log;
        private readonly ObservableConfiguration _config;
        private readonly string _userAgent;
        private readonly string _dataPath;

        private readonly HttpClient _httpClient;

        public CommunityDragonClient(string dataPath, ObservableConfiguration config, string userAgent, RiZhi log)
        {
            _dataPath = dataPath;
            _config = config;
            _userAgent = userAgent;
            _log = log;

            _httpClient = new HttpClient();
        }

        // http client needs to be properly disposed
        ~CommunityDragonClient()
        {
            _httpClient.Dispose();
        }

        public async Task GetRuneStatDescriptions(List<RuneData> runes, string version, string language)
        {
            // ID's of stat runes
            var statRuneIds = new string[6] { "5001", "5002", "5003", "5005", "5007", "5008"};
            // "default" is "en_us" in cdragon
            var cDragonLanguage = language.ToLower() == "en_us" ? "default" : language.ToLower();

            // construct cdragon url
            var url = _config.CommunityDragonBaseUrl
                + GetCommunityDragonVersion(version)
                + "/plugins/rcp-be-lol-game-data/global/"
                + cDragonLanguage
                + "/v1/perks.json";

            var response = await SendGetRequestAsync(url, "application/json");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP request to Community Dragon failed on {(int)response.StatusCode}, {url}");
            }

            using var document = await JsonDocument.ParseAsync(response.Content.ReadAsStream());

            // parse cdragon rune response
            foreach (var cDragonRune in document.RootElement.EnumerateArray())
            {
                var id = cDragonRune.GetProperty("id").GetInt32().ToString();
                // load information for stats runes from cdragon
                if (statRuneIds.Contains(id))
                {
                    runes.Add(new RuneData(id)
                    {
                        Key = cDragonRune.GetProperty("name").GetString()
                            ?? throw new Exception("Get name string returned null"),
                        DisplayName = (cDragonRune.GetProperty("tooltip").GetString()
                            ?? throw new Exception("Get tooltip string returned null"))
                            .Replace("<scaleAD>", "")
                            .Replace("</scaleAD>", "")
                            .Replace("@f1@", "")
                            .Replace("@f2@", "")
                            .Replace("&nbsp;", ""),
                        IconUrl = (cDragonRune.GetProperty("iconPath").GetString()
                            ?? throw new Exception("Get iconPath string returned null"))
                            .Replace("/lol-game-data/assets/v1/", "")
                    });
                }

                // look for rune in our list
                var rune = runes.FirstOrDefault(x => x.Id == id);
                if (rune == null)
                {
                    _log.Warning($"Could not find rune with cDragon id: {id}");
                    continue;
                }

                foreach (var descElement in cDragonRune.GetProperty("endOfGameStatDescs").EnumerateArray())
                {
                    var description = descElement.GetString()
                        ?? throw new Exception("Get description string returned null");

                    description = description.Replace("&nbsp;", " ");
                    description = description.Replace("<speed>", "");
                    description = description.Replace("</speed>", "");

                    // save the description
                    ((List<string>)rune.EndOfGameStatDescs).Add(description);
                }
            }
        }

        private async Task<HttpResponseMessage> SendGetRequestAsync(string url, string accept)
        {
            HttpResponseMessage response;
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            // add headers so server can identify our queries (it's polite to)
            request.Headers.UserAgent.ParseAdd(_userAgent);
            request.Headers.Accept.ParseAdd(accept);
            try
            {
                response = await _httpClient.SendAsync(request);
            }
            catch (HttpRequestException ex)
            {
                _log.Error($"Unable to send HTTP request to {url}, Exception: {ex}");
                throw;
            }

            return response;
        }

        private static string GetCommunityDragonVersion(string version)
        {
            var lastIndex = version.LastIndexOf('.');
            return version[..lastIndex];
        }
    }
}
