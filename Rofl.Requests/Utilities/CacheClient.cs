using Etirps.RiZhi;
using System;
using System.IO;
using Rofl.Requests.Models;

namespace Rofl.Requests.Utilities
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
            string downloadLocation = String.Empty;

            // what kind of request is it?
            switch (request)
            {
                case ChampionRequest c: // check if each unique string isnt null
                    if(String.IsNullOrEmpty(c.ChampionName)) { throw new ArgumentNullException(); }

                    downloadLocation = Path.Combine(CachePath, "champs", $"{c.ChampionName}.png");
                    break;

                case ItemRequest i:
                    if (String.IsNullOrEmpty(i.ItemID)) { throw new ArgumentNullException(); }

                    downloadLocation = Path.Combine(CachePath, "items", $"{i.ItemID}.png");
                    break;

                case MapRequest m:
                    if(String.IsNullOrEmpty(m.MapID)) { throw new ArgumentNullException(); }

                    downloadLocation = Path.Combine(CachePath, "maps", $"{m.MapID}.png");
                    break;

                default:
                    break;
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

        /// <summary>
        /// Given champion name, checks if image exists in cache. If exists, return file path, otherwise return null/empty
        /// </summary>
        /// <param name="champName"></param>
        /// <returns></returns>
        //private Image GetImageFromFile(string filePath)
        //{
        //    if(string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        //    {
        //        return null;
        //    }

        //    // This may throw out of memory exception if the format is wrong
        //    return Image.FromFile(filePath);
        //}
    }
}
