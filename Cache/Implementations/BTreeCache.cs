using System.Collections.Generic;
using System.IO;
using BplusDotNet;

namespace CacheAspect
{
    internal class BTreeCache : ICache
    {
        private static SerializedTree _treeCache;
        private static string _datafile;
        private static string _treefile;
        private readonly List<string> _keys = new List<string>();

        public BTreeCache()
        {
            _datafile = CacheService.DiskPath + "datafile";
            _treefile = CacheService.DiskPath + "treefile";
            LoadCache();
        }

        public object this[string key]
        {
            get
            {
                return _treeCache.ContainsKey(key) ? _treeCache[key] : null;
            }
            set
            {
                lock (_keys)
                {
                    _keys.Add(key);
                }
                _treeCache[key] = value;
                SaveCache();
            }
        }


        public bool Contains(string key)
        {
            return _treeCache.ContainsKey(key);
        }

        public void Delete(string key)
        {
            lock (_keys)
            {
                _keys.Remove(key);
            }
            _treeCache.RemoveKey(key);
            SaveCache();
        }

        public void Clear()
        {
            var key = _treeCache.FirstKey();
            while (!string.IsNullOrWhiteSpace(key))
            {
                Delete(key);
                key = _treeCache.FirstKey();
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                lock (_keys)
                {
                    return _keys.ToArray();
                }
            }
        }

        ~BTreeCache()
        {
            CloseCache();
        }

        public static void LoadCache()
        {
            if (_treeCache != null) return;

            if (File.Exists(_treefile) && File.Exists(_datafile))
            {
                _treeCache = new SerializedTree(hBplusTreeBytes.ReOpen(_treefile, _datafile));
            }
            else
            {
                _treeCache = new SerializedTree(hBplusTreeBytes.Initialize(_treefile, _datafile, 500));
            }
            _treeCache.SetFootPrintLimit(10);
        }

        public static void SaveCache()
        {
            if (_treeCache != null)
            {
                _treeCache.Commit();
            }
        }

        public static void CloseCache()
        {
            _treeCache.Shutdown();
        }
    }
}