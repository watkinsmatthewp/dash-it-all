using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace DashItAll.ConsoleApp.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson<T>(this T obj, bool serializeEnumsAsStrings = true, bool indent = false, bool serializeNulls = false)
        {
            var options = new JsonSerializerSettings
            {
                Formatting = indent ? Formatting.Indented : Formatting.None,
                NullValueHandling = serializeNulls ? NullValueHandling.Include : NullValueHandling.Ignore
            };

            if (serializeEnumsAsStrings)
            {
                options.Converters.Add(new StringEnumConverter());
            }

            return JsonConvert.SerializeObject(obj, options);
        }

        public static T ParseJson<T>(this string value) => JsonConvert.DeserializeObject<T>(value);
    }
}
