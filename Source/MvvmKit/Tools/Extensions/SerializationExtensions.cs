﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class SerializationExtensions
    {
        private static JsonSerializerSettings _settings;

        static SerializationExtensions()
        {
            _settings = new JsonSerializerSettings();
            _settings.Converters.Add(new StringEnumConverter());
        }

        public static string ToJson(this object source)
        {
            var type = source.GetType();
            return JsonConvert.SerializeObject(source, type:type, 
                settings: _settings, formatting: Formatting.Indented);
        }

        public static JToken ToJsonToken(this object source)
        {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new StringEnumConverter());
            var res = JToken.FromObject(source, serializer);
            return res;
        }

        public static string ToJson<T>(this T source) {
            return JsonConvert.SerializeObject(source, Formatting.Indented, _settings);
        }

        public static T FromJson<T>(this string source)
        {
            return JsonConvert.DeserializeObject<T>(source);
        }
    }
}
