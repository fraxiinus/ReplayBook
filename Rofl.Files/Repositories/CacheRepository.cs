using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Rofl.Files.Models;
using Rofl.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Rofl.Files.Repositories
{
    public class CacheRepository
    {
        private string _indexVersion = "0.0.1";
        private Scribe _log;
        private IConfiguration _config;

        private CacheIndex _cacheIndex;

        private string _cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");
        private string _cacheItemPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", "replays");

        private string _myName;

        private bool _dirty;

        public CacheRepository(IConfiguration config, Scribe log)
        {
            _config = config;
            _log = log;
            _myName = this.GetType().ToString();

            // Item cache directory
            if (!Directory.Exists(_cacheItemPath))
            {
                _log.Info(_myName, "Cache item folder does not exist, creating");
                Directory.CreateDirectory(_cacheItemPath);
            }

            // Open cache index
            string cacheIndexPath = Path.Combine(_cachePath, "replaycacheindex.json");
            if (File.Exists(cacheIndexPath))
            {
                _log.Info(_myName, "Replay cache file found, reading...");
                using (StreamReader file = File.OpenText(cacheIndexPath))
                {
                    var serializer = new JsonSerializer();
                    _cacheIndex = serializer.Deserialize(file, typeof(CacheIndex)) as CacheIndex;
                }
            }
            else if (!Directory.Exists(_cachePath))
            {
                _log.Info(_myName, "Cache folder does not exist, creating");
                Directory.CreateDirectory(_cachePath);
                _cacheIndex = new CacheIndex
                {
                    IndexVersion = _indexVersion,
                    Items = new List<CacheIndexItem>()
                };
            }
            else
            {
                _log.Info(_myName, "Replay cache file does not exist, will create on exit");
                _cacheIndex = new CacheIndex
                {
                    IndexVersion = _indexVersion,
                    Items = new List<CacheIndexItem>()
                };
            }
        }

        ~CacheRepository()
        {
            if (_dirty)
            {
                _log.Info(_myName, "Cache is dirty, writing changes...");

                string cacheIndexPath = Path.Combine(_cachePath, "replaycacheindex.json");
                using (StreamWriter file = File.CreateText(cacheIndexPath))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, _cacheIndex);
                }
            }
        }

        /// <summary>
        /// Returns cache item if exists, null if otherwise.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public FileResult CheckCache(string path)
        {
            CacheIndexItem result = _cacheIndex.Items.Where(x => x.Location == path).FirstOrDefault();

            if (result == null)
            {
                return null;
            }

            // Try to open cached file
            string itemPath = Path.Combine(_cacheItemPath, result.Id + ".json");
            if (!File.Exists(itemPath))
            {
                _log.Warning(_myName, "Item in index does not actually exist, deleting index entry");
                _cacheIndex.Items.Remove(result);
                _dirty = true; // save when exit
                return null; // Tell caller to re-parse the file
            }

            // Deserialize cache file and return
            using (StreamReader file = File.OpenText(itemPath))
            {
                var serializer = new JsonSerializer();
                FileResult item = serializer.Deserialize(file, typeof(FileResult)) as FileResult;
                // if it's from cache, it isnt new
                item.IsNewFile = false;
                // duplicate files use the same cache item. Make them properly show correct file
                item.FileInfo.Path = path;
                item.ReplayFile.Location = path;
                item.ReplayFile.Name = Path.GetFileNameWithoutExtension(path);
                return item;
            }
        }

        public void AddCache(FileResult result)
        {
            CacheIndexItem newItem = new CacheIndexItem
            {
                Id = result.ReplayFile.MatchId.ToString(),
                CacheDateTime = DateTimeOffset.UtcNow,
                Location = result.FileInfo.Path
            };

            if (!_cacheIndex.Items.Contains(newItem))
            {
                string itemPath = Path.Combine(_cacheItemPath, newItem.Id + ".json");
                // Cache item exists, but for some reason not indexed
                if (File.Exists(itemPath))
                {
                    _log.Info(_myName, "Item exists, but not in index, adding entry");
                    _cacheIndex.Items.Add(newItem);
                    _dirty = true; // save when exit
                    return;
                }

                _cacheIndex.Items.Add(newItem);
                _dirty = true; // save when exit

                // Write the file result
                using (StreamWriter file = File.CreateText(itemPath))
                {
                    _log.Info(_myName, "Writing cache item...");
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, result);
                }
            }
            else
            {
                _log.Info(_myName, $"Item {result.FileInfo.Path} already exists in cache");
            }
        }
    }
}
