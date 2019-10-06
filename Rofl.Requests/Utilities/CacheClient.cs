using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Rofl.Requests.Models;
using System.Drawing;
using Microsoft.Extensions.Configuration;
using Rofl.Logger;

namespace Rofl.Requests.Utilities
{
    public class CacheClient
    {
        private string _cachePath { get; set; }

        private Scribe _log;

        private string _myName;

        public CacheClient(string cachePath, Scribe log)
        {
            _log = log;
            _cachePath = cachePath;
            _myName = this.GetType().ToString();
        }

        public ResponseBase CheckImageCache(RequestBase request)
        {
            string downloadLocation = String.Empty;

            // what kind of request is it?
            switch (request)
            {
                case ChampionRequest c: // check if each unique string isnt null
                    if(String.IsNullOrEmpty(c.ChampionName)) { throw new ArgumentNullException(); }

                    downloadLocation = Path.Combine(_cachePath, "champs", $"{c.ChampionName}.png");
                    break;

                case ItemRequest i:
                    if (String.IsNullOrEmpty(i.ItemID)) { throw new ArgumentNullException(); }

                    downloadLocation = Path.Combine(_cachePath, "items", $"{i.ItemID}.png");
                    break;

                case MapRequest m:
                    if(String.IsNullOrEmpty(m.MapID)) { throw new ArgumentNullException(); }

                    downloadLocation = Path.Combine(_cachePath, "maps", $"{m.MapID}.png");
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
                _log.Info(_myName, $"Cache miss on {downloadLocation}");
                response.IsFaulted = true;
                response.Exception = new FileNotFoundException("Cache miss", downloadLocation);
                return response;
            }

            //// Get the image
            //try
            //{
            //    response.ResponseImage = GetImageFromFile(downloadLocation);
            //}
            //catch (OutOfMemoryException ex)
            //{
            //    // If the file is not formatted as an image, return an exception
            //    _log.Warning(_myName, $"Image is not a valid format {downloadLocation}");
            //    response.IsFaulted = true;
            //    response.Exception = new Exception("Image is not a valid format", ex);
            //    return response;
            //}
            
            response.ResponsePath = downloadLocation;
            response.IsFaulted = false;
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
