using Newtonsoft.Json;
using Rofl.Files.Models;
using Rofl.Logger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Rofl.Files.Repositories
{
    public class CacheRepository
    {
        private readonly string _indexVersion = "0.0.1";
        private readonly Scribe _log;
        private readonly CacheIndex _cacheIndex;

        private readonly string _cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");
        private readonly string _cacheItemPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache", "replays");
        private readonly string _myName;

        private bool _dirty;

        public CacheRepository(Scribe log)
        {
            _log = log;
            _myName = this.GetType().ToString();

            // Item cache directory
            if (!Directory.Exists(_cacheItemPath))
            {
                _log.Information(_myName, "Cache item folder does not exist, creating");
                Directory.CreateDirectory(_cacheItemPath);
            }

            // Open cache index
            string cacheIndexPath = Path.Combine(_cachePath, "replaycacheindex.json");
            if (File.Exists(cacheIndexPath))
            {
                _log.Information(_myName, "Replay cache file found, reading...");
                using (StreamReader file = File.OpenText(cacheIndexPath))
                {
                    var serializer = new JsonSerializer();
                    _cacheIndex = serializer.Deserialize(file, typeof(CacheIndex)) as CacheIndex;
                }
            }
            else if (!Directory.Exists(_cachePath))
            {
                _log.Information(_myName, "Cache folder does not exist, creating");
                Directory.CreateDirectory(_cachePath);
                _cacheIndex = new CacheIndex
                {
                    IndexVersion = _indexVersion,
                    Items = new List<CacheIndexItem>()
                };
            }
            else
            {
                _log.Information(_myName, "Replay cache file does not exist, will create on exit");
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
                _log.Information(_myName, "Cache is dirty, writing changes...");

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
                var serializer = JsonSerializer.Create();
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
            if(result == null) { throw new ArgumentNullException(nameof(result)); }
            CacheIndexItem newItem = new CacheIndexItem
            {
                Id = result.ReplayFile.MatchId.ToString(CultureInfo.InvariantCulture),
                CacheDateTime = DateTimeOffset.UtcNow,
                Location = result.FileInfo.Path
            };

            if (!_cacheIndex.Items.Contains(newItem))
            {
                string itemPath = Path.Combine(_cacheItemPath, newItem.Id + ".json");
                // Cache item exists, but for some reason not indexed
                if (File.Exists(itemPath))
                {
                    _log.Information(_myName, "Item exists, but not in index, adding entry");
                    _cacheIndex.Items.Add(newItem);
                    _dirty = true; // save when exit
                    return;
                }

                _cacheIndex.Items.Add(newItem);
                _dirty = true; // save when exit

                // Write the file result
                using (StreamWriter file = File.CreateText(itemPath))
                {
                    _log.Information(_myName, "Writing cache item...");
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, result);
                }
            }
            else
            {
                _log.Information(_myName, $"Item {result.FileInfo.Path} already exists in cache");
            }
        }
    }
}
