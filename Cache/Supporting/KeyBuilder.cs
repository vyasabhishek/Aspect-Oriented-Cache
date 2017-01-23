using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using PostSharp.Aspects;

namespace CacheAspect.Supporting
{
    [Serializable]
    public class KeyBuilder
    {
        public string MethodName { get; set; }
        public CacheSettings Settings { get; set; }
        public string GroupName { get; set; }
        public string ParameterProperty { get; set; }
        private Dictionary<int, string> _parametersNameValueMapper;
        private ParameterInfo[] _methodParameters;
        public ParameterInfo[] MethodParameters
        {
            get { return _methodParameters; }
            set { 
                _methodParameters = value;
                TransformParametersIntoNameValueMapper(_methodParameters);
            }
        }

        private void TransformParametersIntoNameValueMapper(IReadOnlyList<ParameterInfo> methodParameters)
        {
            _parametersNameValueMapper = new Dictionary<int, string>();
            for (var i = 0; i < methodParameters.Count(); i++)
            {
                _parametersNameValueMapper.Add(i, methodParameters[i].Name);
            }
        }

        public string BuildCacheKey(object instance, Arguments arguments)
        {
            StringBuilder cacheKeyBuilder = new StringBuilder();

            // start building a key based on the method name if a group name not set
            cacheKeyBuilder.Append(string.IsNullOrWhiteSpace(GroupName) ? MethodName : GroupName);

            if (instance != null)
            {
                cacheKeyBuilder.Append(instance);
                cacheKeyBuilder.Append(";");
            }


            int argIndex;
            switch (Settings)
            {
                case CacheSettings.EntireGroup:
                    return GroupName;
                case CacheSettings.IgnoreParameters:
                    return cacheKeyBuilder.ToString();
                    
                case CacheSettings.UseId:
                    argIndex = GetArgumentIndexByName("Id");
                    cacheKeyBuilder.Append(arguments.GetArgument(argIndex) ?? "Null");
                    break;
                case CacheSettings.UseProperty:
                    argIndex = GetArgumentIndexByName(ParameterProperty);
                    cacheKeyBuilder.Append(arguments.GetArgument(argIndex) ?? "Null");
                    break;
                case CacheSettings.Default:
                    for (var i = 0; i < arguments.Count; i++)
                    {
                        BuildDefaultKey(arguments.GetArgument(i), cacheKeyBuilder);
                    }
                    break;
            }

            return cacheKeyBuilder.ToString();
        }

        private static void BuildDefaultKey(object argument, StringBuilder cacheKeyBuilder)
        {
            var os = argument as ICollection;
            if (os != null)
            {
                cacheKeyBuilder.Append("{");
                foreach (var o in os)
                {
                    cacheKeyBuilder.Append(o ?? "Null");
                }
                cacheKeyBuilder.Append("}");
            }
            else if (argument != null)
            {
                cacheKeyBuilder.Append(argument);
            }
            else
            {
                cacheKeyBuilder.Append("Null");
            }
        }

        private int GetArgumentIndexByName(string paramName)
        {
            var paramKeyValue = _parametersNameValueMapper.SingleOrDefault( arg => string.Compare(arg.Value, paramName, CultureInfo.InvariantCulture, 
                CompareOptions.IgnoreCase) == 0);

            return paramKeyValue.Key;

        }

        private static MethodInfo GetMethod(IReflect toSearch, string methodName, Type returnType, BindingFlags bindingFlags)
        {
            return Array.Find(toSearch.GetMethods(bindingFlags), inf => inf.Name == methodName && inf.ReturnType == returnType);
        }

        public static object DynamicCast(object o, Type ot)
        {
            var method = GetMethod(o.GetType(), "op_Implicit", ot, BindingFlags.Static | BindingFlags.Public) ??
                         GetMethod(o.GetType(), "op_Explicit", ot, BindingFlags.Static | BindingFlags.Public);

            if (method == null) throw new InvalidCastException("Invalid Cast.");

            return method.Invoke(null, new[] { o });
        }
    }
}
