using System;
using System.IO;
using System.Threading.Tasks;
using Rofl.Requests.Models;
using Rofl.Logger;
using System.Net.Http;
using Rofl.Settings.Models;

namespace Rofl.Requests.Utilities
{
    public class DownloadClient
    {
        private readonly string _downloadRootFolder;
        private readonly ObservableSettings _settings;
        private readonly Scribe _log;
        private readonly string _myName;

        public DownloadClient(string downloadPath, ObservableSettings settings, Scribe log)
        {
            if (string.IsNullOrEmpty(downloadPath))
            {
                throw new ArgumentNullException(nameof(downloadPath));
            }

            _settings = settings;
            _log = log;
            _downloadRootFolder = downloadPath;
            _myName = this.GetType().ToString();
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
            if(request == null) { throw new ArgumentNullException(nameof(request)); }

            if (String.IsNullOrEmpty(request.DataDragonVersion))
            {
                throw new ArgumentNullException(nameof(request));
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

                    downloadUrl = _settings.DataDragonBaseUrl + c.DataDragonVersion + _settings.ChampionRelativeUrl + c.ChampionName + ".png";
                    downloadLocation = Path.Combine(_downloadRootFolder, "champs", $"{c.ChampionName}.png");
                    break;

                case ItemRequest i:
                    if (i.ItemID.Equals("0", StringComparison.OrdinalIgnoreCase))
                    {
                        return new ResponseBase()
                        {
                            DataVersion = request.DataDragonVersion,
                            Request = request,
                            IsFaulted = false,
                            RequestUrl = downloadUrl,
                            ResponseDate = DateTime.Now,
                            ResponsePath = null
                        };
                    }

                    downloadUrl = _settings.DataDragonBaseUrl + i.DataDragonVersion + _settings.ItemRelativeUrl + i.ItemID + ".png";
                    downloadLocation = Path.Combine(_downloadRootFolder, "items", $"{i.ItemID}.png");
                    break;

                case MapRequest m:
                    downloadUrl = _settings.DataDragonBaseUrl + m.DataDragonVersion + _settings.MapRelativeUrl + m.MapID + ".png";
                    downloadLocation = Path.Combine(_downloadRootFolder, "maps", $"{m.MapID}.png");
                    break;

                default:
                    break;
            }

            string filePath = await DownloadImage(downloadUrl, downloadLocation).ConfigureAwait(true);
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
                    response = await client.GetAsync(url).ConfigureAwait(true);

                }
                catch (HttpRequestException)
                {
                    _log.Error(_myName, $"Unable to send HTTP request {url}");
                    return null;
                }

                if (response.IsSuccessStatusCode)
                {
                    _log.Information(_myName, $"Made successful HTTP request {url}");
                    using (Stream s = await response.Content.ReadAsStreamAsync().ConfigureAwait(true))
                    {
                        // Creates or overwrites the file
                        if (!Directory.Exists(Path.GetDirectoryName(location)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(location));
                        }
                        using (var file = File.Create(location))
                        {
                            await s.CopyToAsync(file).ConfigureAwait(true);
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
