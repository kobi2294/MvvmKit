using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmAppSample
{
    public static class GlobalRegions
    {
        public static Region ShellWindow { get; } = new Region()
            .WithBehavior(new OpenWindowRegionBehavior());

        // Add your global regions here
    }
}
