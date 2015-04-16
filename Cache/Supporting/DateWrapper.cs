using System;

namespace CacheAspect.Supporting
{
    [Serializable]
    public class DateWrapper<T>
    {
        public T Object
        {
            get;
            set;
        }
        public DateTime Timestamp
        {
            get;
            set;
        }
    }
}
