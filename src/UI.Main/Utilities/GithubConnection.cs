using Etirps.RiZhi;
using System;
using System.Net.Http;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Fraxiinus.ReplayBook.UI.Main.Utilities;

public static class GithubConnection
{
    private const string GithubUrl = @"https://api.github.com/repos/fraxiinus/ReplayBook/releases/latest";

    public static async Task<bool> CheckForUpdate(RiZhi log)
    {
        string latestVersion;
        try
        {
            log.Information("Checking for updates...");
            latestVersion = await GetLatestVersion().ConfigureAwait(true);
        }
        catch (HttpRequestException ex)
        {
            log.Warning("Failed to check for updates - " + ex.ToString());
            return false;
        }

        if (string.IsNullOrEmpty(latestVersion))
        {
            log.Warning("Failed to check for updates - GitHub returned nothing or error code");
            return false;
        }

        AssemblyName assemblyName = Assembly.GetEntryAssembly()?.GetName();
        string assemblyVersion = assemblyName.Version.ToString(3);

        // If version does not match, there is an update
        return !latestVersion.Equals(assemblyVersion, StringComparison.OrdinalIgnoreCase);            
    }

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

            var responseObject = JsonNode.Parse(json) as JsonObject;
            if (responseObject.TryGetPropertyValue("tag_name", out var versionTag))
            {
                return versionTag.ToString();
            }
            return null;
        }
        else
        {
            return null;
        }
    }
}
