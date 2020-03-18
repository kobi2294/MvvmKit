using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class SerializationExtensions
    {
        public static string ToJson<T>(this T source) {
            return JsonConvert.SerializeObject(source, Formatting.Indented);
        }

        public static T FromJson<T>(this string source)
        {
            return JsonConvert.DeserializeObject<T>(source);
        }
    }
}
