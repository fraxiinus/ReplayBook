using Etirps.RiZhi;
using System;
using System.IO;
using Fraxiinus.ReplayBook.Requests.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Fraxiinus.ReplayBook.Requests.Utilities
{
    public class CacheClient
    {
        private string CachePath { get; set; }

        private readonly RiZhi _log;

        public CacheClient(string cachePath, RiZhi log)
        {
            _log = log;
            CachePath = cachePath;
        }

        public ResponseBase CheckImageCache(RequestBase request)
        {
            if (request is null) { throw new ArgumentNullException(nameof(request)); }
            string downloadLocation = string.Empty;

            // what kind of request is it?
            switch (request)
            {
                case ChampionRequest c: // check if each unique string isnt null
                    if (string.IsNullOrEmpty(c.ChampionName)) { throw new ArgumentNullException(nameof(request)); }

                    downloadLocation = Path.Combine(CachePath, "champs", $"{c.ChampionName}.png");
                    break;

                case ItemRequest i:
                    if (string.IsNullOrEmpty(i.ItemID)) { throw new ArgumentNullException(nameof(request)); }

                    downloadLocation = Path.Combine(CachePath, "items", $"{i.ItemID}.png");
                    break;

                case MapRequest m:
                    if (string.IsNullOrEmpty(m.MapID)) { throw new ArgumentNullException(nameof(request)); }

                    downloadLocation = Path.Combine(CachePath, "maps", $"{m.MapID}.png");
                    break;
                case RuneRequest r:
                    if (string.IsNullOrEmpty(r.TargetPath)) { throw new ArgumentNullException(nameof(request)); }

                    downloadLocation = Path.Combine(CachePath, "runes", $"{r.RuneKey}.png");
                    break;
                default:
                    throw new NotSupportedException($"unsupported request type: {request.GetType()}");
            }

            // Create the response item
            ResponseBase response = new ResponseBase
            {
                Request = request
            };

            // Does the file already exist in that location?
            if (!File.Exists(downloadLocation))
            {
                // _log.Information($"Cache miss on {downloadLocation}");
                response.IsFaulted = true;
                response.Exception = new FileNotFoundException("Cache miss", downloadLocation);
                return response;
            }

            response.ResponsePath = downloadLocation;
            response.IsFaulted = false;
            response.FromCache = true;
            return response;
        }

        public async Task ClearImageCache(string path)
        {
            var dirInfo = new DirectoryInfo(path);

            // short circuit if directory does not exist
            if (!dirInfo.Exists) return;

            await Task.Run(() =>
            {
                foreach (var image in dirInfo.EnumerateFiles("*.png"))
                {
                    _log.Debug("Deleting cache image: " + image.FullName);

                    File.Delete(image.FullName);
                }
            }).ConfigureAwait(true);
        }

        //public async Task ClearChampsImageCache()
        //{
        //    var dirInfo = new DirectoryInfo(Path.Combine(CachePath, "champs"));
        //    await Task.Run(() =>
        //    {
        //        foreach (var image in dirInfo.EnumerateFiles("*.png"))
        //        {
        //            _log.Debug("Deleting cache image" + image.FullName);

        //            File.Delete(image.FullName);
        //        }
        //    }).ConfigureAwait(true);
        //}
    }
}
