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
    }
}
