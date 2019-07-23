using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Rofl.Requests.Models;
using System.Drawing;

namespace Rofl.Requests.Utilities
{
    public class CacheClient
    {
        public string DownloadRootPath { get; set; }

        public CacheClient(string downloadPath)
        {
            DownloadRootPath = downloadPath;
        }

        public ResponseBase CheckImageCache(RequestBase request)
        {
            string downloadLocation = String.Empty;

            switch (request)
            {
                case ChampionRequest c:
                    if(String.IsNullOrEmpty(c.ChampionName)) { throw new ArgumentNullException(); }

                    downloadLocation = Path.Combine(DownloadRootPath, "champs", $"{c.ChampionName}.png");
                    break;

                case ItemRequest i:
                    if (String.IsNullOrEmpty(i.ItemID)) { throw new ArgumentNullException(); }

                    downloadLocation = Path.Combine(DownloadRootPath, "items", $"{i.ItemID}.png");
                    break;

                case MapRequest m:
                    if(String.IsNullOrEmpty(m.MapID)) { throw new ArgumentNullException(); }

                    downloadLocation = Path.Combine(DownloadRootPath, "maps", $"{m.MapID}.png");
                    break;

                default:
                    break;
            }

            ResponseBase response = new ResponseBase
            {
                Request = request
            };

            if (!File.Exists(downloadLocation))
            {
                response.IsFaulted = true;
                response.Exception = new FileNotFoundException("Cache miss", downloadLocation);
                return response;
            }

            response.ResponseImage = GetImageFromFile(downloadLocation);
            response.ResponsePath = downloadLocation;
            response.IsFaulted = false;
            return response;
        }

        /// <summary>
        /// Given champion name, checks if image exists in cache. If exists, return file path, otherwise return null/empty
        /// </summary>
        /// <param name="champName"></param>
        /// <returns></returns>
        private Image GetImageFromFile(string filePath)
        {
            if(string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return null;
            }

            return Image.FromFile(filePath);
        }
    }
}
