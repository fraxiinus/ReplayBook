using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Fraxiinus.ReplayBook.StaticData.Models
{
    public class BundleIndex
    {
        [JsonPropertyName("downloaded_bundles")]
        public Dictionary<string, string> DownloadedBundles { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("patch_versions")]
        public List<string> PatchVersions { get; set; } = new List<string>();

        [JsonPropertyName("last_patch_fetch")]
        public DateTimeOffset LastPatchFetch { get; set; }
    }
}
