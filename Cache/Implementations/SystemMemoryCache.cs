using System.Collections.Generic;
using System.Runtime.Caching;

namespace CacheAspect
{
    public class SystemMemoryCache : ICache
    {
        private readonly List<string> _keys = new List<string>();

        public object this[string key]
        {
            get { return MemoryCache.Default[key]; }
            set
            {
                lock (_keys)
                {
                    _keys.Add(key);
                }
                MemoryCache.Default[key] = value;
            }
        }

        public bool Contains(string key)
        {
            return MemoryCache.Default.Contains(key);
        }

        public void Delete(string key)
        {
            lock (_keys)
            {
                _keys.Remove(key);
            }
            MemoryCache.Default.Remove(key);
        }

        public void Clear()
        {
            foreach (KeyValuePair<string, object> kvp in MemoryCache.Default)
            {
                MemoryCache.Default.Remove(kvp.Key);
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
    }
}
