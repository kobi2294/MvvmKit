using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class DictionaryExtensions
    {
        public static T GetOrCreate<K, T>(this Dictionary<K, T> source, K key, Func<T> factory)
        {
            if (source.TryGetValue(key, out T value))
            {
                return value;
            }

            var res = factory();
            source.Add(key, res);
            return res;
        }

        public static T GetOrDefault<K, T>(this Dictionary<K, T> source, K key)
        {
            if (source.TryGetValue(key, out T value))
            {
                return value;
            }

            return default(T);
        }
    }
}
