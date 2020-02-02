using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.HeckelDiff
{
    public enum Counter
    {
        Zero, 
        One, 
        Many
    }

    public static class CounterExtensions
    {
        public static Counter Increment(this Counter c)
        {
            var res = Counter.Many;

            if (c == Counter.Zero) res = Counter.One;

            return res;
        }
    }
}
