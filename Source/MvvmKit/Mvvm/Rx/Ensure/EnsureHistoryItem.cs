using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class EnsureHistoryItem: IImmutable
    {
        public string EnsureMethod { get; }

        public object Before { get; set; }

        public object After { get; set; }

        public EnsureContext Context { get; set; }

        public EnsureHistoryItem(
            string ensureMethod = "", 
            object before = null, 
            object after = null, 
            EnsureContext context = null)
        {
            EnsureMethod = ensureMethod;
            Before = before;
            After = after;
            Context = context;
        }


    }
}
