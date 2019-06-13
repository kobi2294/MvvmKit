using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.HeckelDiff
{
    public static class Entries
    {
        public static SymbolEntry<T> Symbol<T>(T item) => new SymbolEntry<T>(item);

        public static IndexEntry Index(int index) => new IndexEntry(index);

        public abstract class Entry
        {
        }

        public class SymbolEntry<T>: Entry, IEquatable<SymbolEntry<T>>
        {
            public T OldItem { get; }

            // Occurences of symbol in Old array
            public Counter OldCounter { get; set; }

            // Occurences of symbol in New array
            public Counter NewCounter { get; set; }

            // Indices of symbol in Old
            public List<int> OldNumbers { get; }

            public bool OccursInBoth => (OldCounter != Counter.Zero) && (NewCounter != Counter.Zero);

            public SymbolEntry(T item)
            {
                OldNumbers = new List<int>();
                OldItem = item;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as SymbolEntry<T>);
            }

            public override int GetHashCode()
            {
                var members = OldCounter.AsIEnumerable<object>()
                    .Concat(OldItem)
                    .Concat(NewCounter)
                    .Concat(OldNumbers);
                return ObjectExtensions.GenerateHashCode(members);
            }

            public bool Equals(SymbolEntry<T> other)
            {
                return (OldCounter == other.OldCounter)
                    && (NewCounter == other.NewCounter)
                    && (Equals(OldItem, other.OldItem))
                    && (OldNumbers.SequenceEqual(other.OldNumbers));
            }

            public static bool operator ==(SymbolEntry<T> s1, SymbolEntry<T> s2)
            {
                var isnull1 = ReferenceEquals(s1, null);
                var isnull2 = ReferenceEquals(s2, null);

                if (isnull1 && isnull2) return true;
                if (isnull1 || isnull2) return false;

                return s1.Equals(s2);
            }

            public static bool operator!=(SymbolEntry<T> s1, SymbolEntry<T> s2)
            {
                return !(s1 == s2);
            }
        }

        public class IndexEntry : Entry
        {
            public int Index { get; }

            public IndexEntry(int index)
            {
                Index = index;
            }
        }
    }
}
