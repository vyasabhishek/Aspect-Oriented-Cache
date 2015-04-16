using System;
using System.Reflection;
using CacheAspect.Supporting;
using PostSharp.Aspects;

namespace CacheAspect
{
    public static partial class Cache
    {
        [Serializable]
        public class Cacheable : OnMethodBoundaryAspect
        {
            private KeyBuilder _keyBuilder;
            public KeyBuilder KeyBuilder
            {
                get { return _keyBuilder ?? (_keyBuilder = new KeyBuilder()); }
            }

            #region Constructors
            
            public Cacheable(String groupName, CacheSettings settings, String parameterProperty)
            {
                KeyBuilder.GroupName = groupName;
                KeyBuilder.Settings = settings;
                KeyBuilder.ParameterProperty = parameterProperty;
            }

            public Cacheable(String groupName, CacheSettings settings = CacheSettings.Default)
                : this(groupName, settings, string.Empty)
            {
            }

            public Cacheable() : this(string.Empty)
            {

            }
            #endregion

            //Method executed at build time.
            public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
            {
                KeyBuilder.MethodParameters = method.GetParameters();
                if (method.DeclaringType != null)
                {
                    KeyBuilder.MethodName = string.Format("{0}.{1}", method.DeclaringType.FullName, method.Name);
                }
            }

            // This method is executed before the execution of target methods of this aspect.
            public override void OnEntry(MethodExecutionArgs args)
            {
                // Compute the cache key.
                string cacheKey = KeyBuilder.BuildCacheKey(args.Instance, args.Arguments);

                // Fetch the value from the cache.
                ICache cache = CacheService.Cache;
                DateWrapper<object> value = (DateWrapper<object>)(cache.Contains(cacheKey) ? cache[cacheKey] : null);

                if (value != null && !IsTooOld(value.Timestamp))
                {
                    // The value was found in cache. Don't execute the method. Return immediately.
                    args.ReturnValue = value.Object;
                    args.FlowBehavior = FlowBehavior.Return;
                }
                else
                {
                    // The value was NOT found in cache. Continue with method execution, but store
                    // the cache key so that we don't have to compute it in OnSuccess.
                    args.MethodExecutionTag = cacheKey;
                }
            }

            // This method is executed upon successful completion of target methods of this aspect.
            public override void OnSuccess(MethodExecutionArgs args)
            {
                string cacheKey = (string)args.MethodExecutionTag;
                CacheService.Cache[cacheKey] = new DateWrapper<Object>()
                {
                    Object = args.ReturnValue,
                    Timestamp = DateTime.UtcNow
                };
            }

            private bool IsTooOld(DateTime time)
            {
                if (KeyBuilder.Settings == CacheSettings.IgnoreTtl)
                {
                    return false;
                }
                return DateTime.UtcNow - time > CacheService.TimeToLive;                
            }

        }
    }

}

