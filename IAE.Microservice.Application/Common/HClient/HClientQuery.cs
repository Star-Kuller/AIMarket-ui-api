using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace IAE.Microservice.Application.Common.HClient
{
    public abstract class HClientQuery
    {
        /// <summary>
        /// <see cref="ToPathAndQuery"/> method supports only next types:
        /// <see cref="long"/>, nullable <see cref="long"/>, <see cref="long"/> array,
        /// <see cref="int"/>, nullable <see cref="int"/>,
        /// <see cref="string"/> and <see cref="string"/> array.
        /// </summary>
        public string ToPathAndQuery(string path)
        {
            var pars = new List<string>();
            foreach (var pInfo in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var obj = pInfo.GetValue(this);
                if (obj == null)
                {
                    continue;
                }

                var keyIgnore = pInfo.GetCustomAttribute<JsonIgnoreAttribute>();
                if (keyIgnore != null)
                {
                    continue;
                }

                var key = pInfo.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName
                          ?? char.ToLower(pInfo.Name[0]) + pInfo.Name.Substring(1);

                #region longs

                if (pInfo.PropertyType == typeof(long) || pInfo.PropertyType == typeof(long?))
                {
                    var value = (long)obj;
                    pars.Add($"{key}={value}");
                }
                else if (pInfo.PropertyType == typeof(long[]))
                {
                    var value = (long[])obj;
                    if (value.Length == 0)
                    {
                        continue;
                    }

                    foreach (var v in value)
                    {
                        pars.Add($"{key}={v}");
                    }
                }

                #endregion

                #region ints

                else if (pInfo.PropertyType == typeof(int) || pInfo.PropertyType == typeof(int?))
                {
                    var value = (int)obj;
                    pars.Add($"{key}={value}");
                }

                #endregion

                #region strings

                else if (pInfo.PropertyType == typeof(string))
                {
                    var value = (string)obj;
                    if (string.IsNullOrWhiteSpace(value)) continue;
                    pars.Add($"{key}={value}");
                }
                else if (pInfo.PropertyType == typeof(string[]))
                {
                    var value = (string[])obj;
                    if (value.Length == 0)
                    {
                        continue;
                    }

                    foreach (var v in value)
                    {
                        if (string.IsNullOrWhiteSpace(v)) continue;
                        pars.Add($"{key}={v}");
                    }
                }

                #endregion

                else
                {
                    throw new TypeLoadException($"Unsupported type for query property: {pInfo.PropertyType.Name}");
                }
            }

            for (var i = 0; i < pars.Count; i++)
            {
                pars[i] = pars[i].Replace("+", "%2B");
            }

            var result = $"{(path.EndsWith('/') ? path.Remove(path.Length - 1) : path)}";
            if (pars.Count > 0)
            {
                result += $"?{string.Join('&', pars)}";
            }

            return result;
        }
    }
}