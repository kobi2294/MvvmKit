using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class DiffResults<T>: IImmutable
    {
        public static DiffResults<T> Empty { get; } = new DiffResults<T>();

        public static DiffResults<T> ResetTo(ImmutableList<T> values)
        {
            return new DiffResults<T>(
                reset: true,
                added: values.Select((t, i) => (at: i, item: t)).ToImmutableList());
        }

        // when true - ignore Moved, Removed, and Modified, just clear the collection and add the Added items
        public bool Reset { get; }

        public ImmutableList<(int from, T item)> Removed { get; }

        public ImmutableList<(int from, int to)> Moved { get; }

        public ImmutableList<(int to, T item)> Added { get; }

        public ImmutableList<(int at, T old, T @new)> Modified { get; }

        public DiffResults(
            bool reset = false, 
            ImmutableList<(int from, T item)> removed = null,
            ImmutableList<(int from, int to)> moved = null,
            ImmutableList<(int at, T item)> added = null,
            ImmutableList<(int at, T old, T @new)> modified = null
            )
        {
            Reset = reset;
            Removed = removed ?? ImmutableList<(int from, T item)>.Empty;
            Moved = moved ?? ImmutableList<(int from, int to)>.Empty;
            Added = added ?? ImmutableList<(int to, T item)>.Empty;
            Modified = modified ?? ImmutableList<(int at, T old, T @new)>.Empty;
        }
    }

}
