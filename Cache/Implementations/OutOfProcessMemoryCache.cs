using System;
using System.Collections.Generic;
using Microsoft.ApplicationServer.Caching;

namespace CacheAspect
{
    public class OutOfProcessMemoryCache : ICache
    {
        private const String CacheName = "CacheAttribute";
        private static DataCache _cache;
        private readonly List<string> _keys = new List<string>();

        public OutOfProcessMemoryCache()
        {
            var configuration = new DataCacheFactoryConfiguration();
            var factory = new DataCacheFactory(configuration);
            _cache = factory.GetCache(CacheName);
        }

        public object this[string key]
        {
            get { return _cache[key]; }
            set
            {
                lock (_keys)
                {
                    _keys.Add(key);
                }
                _cache[key] = value;
            }
        }


        public bool Contains(string key)
        {
            //App Fabric Cache has no Contains method
            return _cache[key] != null;
        }


        public void Delete(string key)
        {
            lock (_keys)
            {
                _keys.Remove(key);
            }
            _cache.Remove(key);
        }

        public void Clear()
        {
            throw new NotImplementedException("Clearing AppFabric cache has not yet been implemented.");
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
    }
}