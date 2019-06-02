using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class ObjectExtensions
    {
        public static T Do<T>(this T source, Action<T> action)
        {
            action(source);
            return source;
        }

        public static int GenerateHashCode(IEnumerable<object> items)
        {
            unchecked
            {
                var first = items.FirstOrDefault();
                var res = first != null ? first.GetHashCode() : 0;

                foreach (var item in items.Skip(1))
                {
                    res *= 397;
                    if (item != null) res = res ^ item.GetHashCode();
                }

                return res;
            }
        }

        public static int GenerateHashCode(params object[] items)
        {
            return GenerateHashCode(items.ToList());
        }
    }
}
