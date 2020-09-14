using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class EnsureSessionHistory
    {
        public object Action { get; }

        public object Before { get; }

        public object After { get; }

        public ImmutableList<EnsureHistoryItem> Items { get; }

        public EnsureSessionHistory(
            object action = null, 
            object before = null, 
            object after = null, 
            ImmutableList<EnsureHistoryItem> items = null)
        {
            Action = action;
            Before = before;
            After = after;
            Items = items ?? ImmutableList<EnsureHistoryItem>.Empty;
        }
    }
}
