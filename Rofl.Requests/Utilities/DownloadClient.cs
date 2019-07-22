using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Rofl.Requests.Models;
using System.Drawing;

namespace Rofl.Requests.Utilities
{
    public class DownloadClient
    {
        public string DownloadRootPath { get; set; }

        public string DataDragonBaseUrl { get; private set; } = @"http://ddragon.leagueoflegends.com/cdn/";

        public string ChampionBaseUrl { get; private set; } = @"/img/champion/";

        public string MapBaseUrl { get; private set; } = @"/img/map/map";

        public string ItemBaseUrl { get; private set; } = @"/img/item/";

        public DownloadClient(string downloadPath)
        {
            DownloadRootPath = downloadPath;
        }

        /// <summary>
        /// Given item id and version strings, downloads image to downloadRoot
        /// </summary>
        /// <param name="champName"></param>
        /// <param name="version"></param>
        /// <param name="downloadRoot"></param>
        /// <returns></returns>
        public async Task<ResponseBase> DownloadIconImageAsync(RequestBase request, string version)
        {
            if (String.IsNullOrEmpty(version) || String.IsNullOrEmpty(DownloadRootPath))
            {
                throw new ArgumentNullException();
            }

            string downloadUrl = String.Empty; //DataDragonBaseUrl + version + ItemBaseUrl + request.ItemID + ".png";
            string downloadLocation = String.Empty; //Path.Combine(DownloadRootPath, "items", $"{request.ItemID}.png");

            switch (request)
            {
                case ChampionRequest c:
                    downloadUrl = DataDragonBaseUrl + version + ChampionBaseUrl + c.ChampionName + ".png";
                    downloadLocation = Path.Combine(DownloadRootPath, "champs", $"{c.ChampionName}.png");
                    break;

                case ItemRequest i:
                    downloadUrl = DataDragonBaseUrl + version + ItemBaseUrl + i.ItemID + ".png";
                    downloadLocation = Path.Combine(DownloadRootPath, "items", $"{i.ItemID}.png");
                    break;

                case MapRequest m:
                    downloadUrl = DataDragonBaseUrl + version + MapBaseUrl + m.MapID + ".png";
                    downloadLocation = Path.Combine(DownloadRootPath, "maps", $"{m.MapID}.png");
                    break;

                default:
                    break;
            }

            Image itemImage = await DownloadImage(downloadUrl);

            SaveImage(itemImage, downloadLocation);

            return new ResponseBase()
            {
                DataVersion = version,
                Request = request,
                IsFaulted = false,
                RequestUrl = downloadUrl,
                ResponseDate = DateTime.Now,
                ResponseImage = itemImage,
                ResponsePath = downloadLocation
            };
        }

        private async Task<Image> DownloadImage(string url)
        {
            using (WebClient wc = new WebClient())
            {
                byte[] rawImage = await wc.DownloadDataTaskAsync(url);

                using (MemoryStream ms = new MemoryStream(rawImage))
                {
                    return Image.FromStream(ms);
                }
            }
        }

        private void SaveImage(Image image, string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            image.Save(path);
        }
    }
}
