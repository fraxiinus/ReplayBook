using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Rofl.Requests.Models;
using System.Drawing;
using Rofl.Logger;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace Rofl.Requests.Utilities
{
    public class DownloadClient
    {
        private string _downloadRootFolder;
        private IConfiguration _config;
        private Scribe _log;
        private string _myName;

        private string _dataDragonBaseUrl;
        private string _championRelUrl;
        private string _mapRelUrl;
        private string _itemRelUrl;

        public DownloadClient(string downloadPath, IConfiguration config, Scribe log)
        {
            _config = config;
            _log = log;
            _downloadRootFolder = downloadPath;
            _myName = this.GetType().ToString();

            _dataDragonBaseUrl = _config.GetSection("downloader:datadragon_baseurl").Value;
            _championRelUrl = _config.GetSection("downloader:champion_relurl").Value;
            _mapRelUrl = _config.GetSection("downloader:map_relurl").Value;
            _itemRelUrl = _config.GetSection("downloader:item_relurl").Value;
        }

        /// <summary>
        /// Given item id and version strings, downloads image to downloadRoot
        /// </summary>
        /// <param name="champName"></param>
        /// <param name="version"></param>
        /// <param name="downloadRoot"></param>
        /// <returns></returns>
        public async Task<ResponseBase> DownloadIconImageAsync(RequestBase request)
        {
            if (String.IsNullOrEmpty(request.DataDragonVersion) || String.IsNullOrEmpty(_downloadRootFolder))
            {
                throw new ArgumentNullException();
            }

            string downloadUrl = String.Empty; //DataDragonBaseUrl + version + ItemBaseUrl + request.ItemID + ".png";
            string downloadLocation = String.Empty; //Path.Combine(DownloadRootPath, "items", $"{request.ItemID}.png");

            switch (request)
            {
                case ChampionRequest c:
                    // Fuck fiddlesticks
                    if (c.ChampionName == "FiddleSticks")
                    {
                        c.ChampionName = "Fiddlesticks";
                    }

                    downloadUrl = _dataDragonBaseUrl + c.DataDragonVersion + _championRelUrl + c.ChampionName + ".png";
                    downloadLocation = Path.Combine(_downloadRootFolder, "champs", $"{c.ChampionName}.png");
                    break;

                case ItemRequest i:
                    if(i.ItemID.Equals("0"))
                    {
                        throw new Exception("empty");
                    }

                    downloadUrl = _dataDragonBaseUrl + i.DataDragonVersion + _itemRelUrl + i.ItemID + ".png";
                    downloadLocation = Path.Combine(_downloadRootFolder, "items", $"{i.ItemID}.png");
                    break;

                case MapRequest m:
                    downloadUrl = _dataDragonBaseUrl + m.DataDragonVersion + _mapRelUrl + m.MapID + ".png";
                    downloadLocation = Path.Combine(_downloadRootFolder, "maps", $"{m.MapID}.png");
                    break;

                default:
                    break;
            }

            string filePath = await DownloadImage(downloadUrl, downloadLocation);
            // failed to download an image!
            if(filePath == null)
            {
                return new ResponseBase()
                {
                    DataVersion = request.DataDragonVersion,
                    Request = request,
                    IsFaulted = true,
                    RequestUrl = downloadUrl,
                    ResponseDate = DateTime.Now,
                    ResponsePath = null
                };
            }

            return new ResponseBase()
            {
                DataVersion = request.DataDragonVersion,
                Request = request,
                IsFaulted = false,
                RequestUrl = downloadUrl,
                ResponseDate = DateTime.Now,
                ResponsePath = filePath
            };
        }

        private async Task<String> DownloadImage(string url, string location)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response;
                try
                {
                    response = await client.GetAsync(url);

                }
                catch (HttpRequestException)
                {
                    _log.Error(_myName, $"Unable to send HTTP request {url}");
                    return null;
                }

                if (response.IsSuccessStatusCode)
                {
                    _log.Info(_myName, $"Made successful HTTP request {url}");
                    using (Stream s = await response.Content.ReadAsStreamAsync())
                    {
                        // Creates or overwrites the file
                        if (!Directory.Exists(Path.GetDirectoryName(location)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(location));
                        }
                        using (var file = File.Create(location))
                        {
                            await s.CopyToAsync(file);
                        }
                    }
                    return location;
                }
                else
                {
                    _log.Warning(_myName, $"HTTP request failed {(int) response.StatusCode} {url}");
                    return null;
                }
            }
        }

        //private void SaveImage(Image image, string path)
        //{
        //    if (!Directory.Exists(Path.GetDirectoryName(path)))
        //    {
        //        Directory.CreateDirectory(Path.GetDirectoryName(path));
        //    }

        //    // Delete if already exists, this happens if the existing file failed to read
        //    if (File.Exists(path))
        //    {
        //        _log.Info(_myName, $"Image already exists, deleting {path}");
        //        File.Delete(path);
        //    }

        //    image.Save(path);
        //}
    }
}
