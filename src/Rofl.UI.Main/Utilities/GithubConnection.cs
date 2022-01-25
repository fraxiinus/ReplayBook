using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rofl.UI.Main.Utilities
{
    public static class GithubConnection
    {
        private const string GithubUrl = @"https://api.github.com/repos/fraxiinus/ReplayBook/releases/latest";

        /// <summary>
        /// Gets latest application version from Github
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetLatestVersion()
        {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(GithubUrl)
            };
            request.Headers.UserAgent.ParseAdd(ApplicationProperties.UserAgent);
            request.Headers.Accept.ParseAdd(@"application/vnd.github.v3+json");

            // Send request
            HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(true);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

                JObject responseObject = JObject.Parse(json);
                return responseObject.ContainsKey("tag_name") ? responseObject["tag_name"].ToString() : null;
            }
            else
            {
                return null;
            }
        }
    }
}
