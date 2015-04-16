using System;
using System.Linq;
using System.Reflection;
using CacheAspect.Supporting;
using PostSharp.Aspects;

namespace CacheAspect
{
    public static partial class Cache
    {
        [Serializable]
        public class TriggerInvalidation : OnMethodBoundaryAspect
        {
            private KeyBuilder _keyBuilder;
            public KeyBuilder KeyBuilder
            {
                get { return _keyBuilder ?? (_keyBuilder = new KeyBuilder()); }
            }

            #region Constructors
            
            public TriggerInvalidation(String groupName, CacheSettings settings, String parameterProperty)
            {
                KeyBuilder.GroupName = groupName;
                KeyBuilder.Settings = settings;
                KeyBuilder.ParameterProperty = parameterProperty;
            }

            public TriggerInvalidation(String groupName, CacheSettings settings)
                : this(groupName, settings, string.Empty)
            {
            }

            public TriggerInvalidation(String groupName)
                : this(groupName, CacheSettings.Default, string.Empty)
            {
            }

            public TriggerInvalidation()
                : this(string.Empty)
            {

            }
            #endregion

            //Method executed at build time.
            public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
            {
                KeyBuilder.MethodParameters = method.GetParameters();
                if (method.DeclaringType != null)
                {
                    KeyBuilder.MethodName =  string.Format("{0}.{1}", method.DeclaringType.FullName, method.Name);
                }
            }

            public override void OnExit(MethodExecutionArgs args)
            {
                var key = KeyBuilder.BuildCacheKey(args.Instance, args.Arguments);

                if (KeyBuilder.Settings == CacheSettings.EntireGroup)
                {
                    var keys = CacheService.Cache.Keys;

                    foreach (var k in keys.Where(x => x.StartsWith(key)))
                    {
                        CacheService.Cache.Delete(k);
                    }
                }
                else if (CacheService.Cache.Contains(key))
                {
                    CacheService.Cache.Delete(key);
                }

                base.OnExit(args);
            }


        }
    }
}

