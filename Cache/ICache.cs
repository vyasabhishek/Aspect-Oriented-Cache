using System.Collections.Generic;

namespace CacheAspect
{
    public interface ICache
    {
        object this[string key] { get; set; }

        bool Contains(string key);

        void Delete(string key);

        void Clear();

        IEnumerable<string> Keys { get; }
    }
}
