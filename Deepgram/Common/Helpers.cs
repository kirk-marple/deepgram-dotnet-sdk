﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/* Unmerged change from project 'Deepgram (netstandard2.0)'
Before:
using Newtonsoft.Json.Linq;
After:
using System.Web;
*/
using System.Web;

namespace Deepgram.Common
{
    internal static class Helpers
    {
        public static string GetUserAgent()
        {
#if NETSTANDARD1_6 || NETSTANDARD2_0 || NETSTANDARD2_1
            // TODO: watch the next core release; may have functionality to make this cleaner
            var languageVersion = (System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription)
                .Replace(" ", string.Empty)
                .Replace("/", string.Empty)
                .Replace(":", string.Empty)
                .Replace(";", string.Empty)
                .Replace("_", string.Empty)
                .Replace("(", string.Empty)
                .Replace(")", string.Empty)
                ;
#else
            var languageVersion = System.Diagnostics.FileVersionInfo
                .GetVersionInfo(typeof(int).Assembly.Location)
                .ProductVersion;
#endif
            var libraryVersion = typeof(Helpers)
                .GetTypeInfo()
                .Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;

            return $"deepgram/{libraryVersion} dotnet/{languageVersion}";
        }

        public static string GetParameters(object parameters = null)
        {
            List<KeyValuePair<string, string>> paramList = new List<KeyValuePair<string, string>>();

            if (parameters != null)
            {

                var json = JsonConvert.SerializeObject(parameters);
                var jObj = (JObject)JsonConvert.DeserializeObject(json);

                foreach (var prop in jObj.Properties())
                {
                    if (prop.HasValues && !String.IsNullOrEmpty(prop.Value.ToString()))
                    {
                        if (prop.Value.Type == JTokenType.Array)
                        {
                            foreach (var value in prop.Values())
                            {
                                paramList.Add(new KeyValuePair<string, string>(prop.Name, HttpUtility.UrlEncode(value.ToString())));
                            }
                        }
                        else if (prop.Value.Type == JTokenType.Date)
                        {
                            paramList.Add(new KeyValuePair<string, string>(prop.Name, HttpUtility.UrlEncode(((DateTime)prop.Value).ToString("yyyy-MM-dd"))));
                        }
                        else
                        {
                            paramList.Add(new KeyValuePair<string, string>(prop.Name, HttpUtility.UrlEncode(prop.Value.ToString())));
                        }
                    }
                }

            }

            return String.Join("&", paramList.Select(s => $"{s.Key}={s.Value.ToString()}")).ToLower();
        }
    }
}
