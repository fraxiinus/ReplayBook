using System.Text.Json.Serialization;

namespace Fraxiinus.ReplayBook.StaticData.Models
{
    public class BundleIndex
    {
        /// <summary>
        /// Patch number to bundle index file
        /// </summary>
        [JsonPropertyName("downloaded_bundles")]
        public Dictionary<string, string> DownloadedBundles { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// List of all known patch versions available to download
        /// </summary>
        [JsonPropertyName("patch_versions")]
        public List<string> PatchVersions { get; set; } = new List<string>();

        /// <summary>
        /// Last time patch versions were fetched
        /// </summary>
        [JsonPropertyName("last_patch_fetch")]
        public DateTimeOffset LastPatchFetch { get; set; }
    }
}
