using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class TimespanExtensions
    {
        public static TimeSpan Milliseconds(this int source)
        {
            return TimeSpan.FromMilliseconds(source);
        }

        public static TimeSpan Seconds(this int source)
        {
            return TimeSpan.FromSeconds(source);
        }

        public static TimeSpan Minutes(this int source)
        {
            return TimeSpan.FromMinutes(source);
        }

        public static TimeSpan Hours(this int source)
        {
            return TimeSpan.FromHours(source);
        }

    }
}
