using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class StringExtensions
    {
        public static bool HasAnyText(this string source)
        {
            return !string.IsNullOrWhiteSpace(source);
        }

        public static bool IsNullOrWhiteSpace(this string source)
        {
            return string.IsNullOrWhiteSpace(source);
        }

        public static string SubstringUntil(this string source, string postfix)
        {
            var index = source.IndexOf(postfix);
            if (index < 0) return source;
            return source.Substring(0, index);
        }

        public static string SubstringUntil(this string source, char postfix)
        {
            var index = source.IndexOf(postfix);
            if (index < 0) return source;
            return source.Substring(0, index);
        }

        public static string Join(this IEnumerable<string> source, string seperator)
        {
            return string.Join(seperator, source);
        }

    }
}
