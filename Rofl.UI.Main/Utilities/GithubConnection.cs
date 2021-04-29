using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.UI.Main.Utilities
{
    public static class GithubConnection
    {
        private const string UserAgent = @"ReplayBook/R1.2.1 (+https://github.com/fraxiinus/ReplayBook)";
        private const string GithubUrl = @"https://api.github.com/repos/fraxiinus/ReplayBook/releases/latest";

        public static async Task<string> GetLatestVersion()
        {
            using (var client = new HttpClient())
            { 
                using (var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(GithubUrl)
                })
                {
                    request.Headers.UserAgent.ParseAdd(UserAgent);
                    request.Headers.Accept.ParseAdd(@"application/vnd.github.v3+json");

                    var response = await client.SendAsync(request).ConfigureAwait(true);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

                        var responseObject = JObject.Parse(json);
                        if (responseObject.ContainsKey("tag_name"))
                        {
                            return responseObject["tag_name"].ToString();
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}
