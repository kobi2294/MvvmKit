using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class ListReset: ListChange, IEquatable<ListReset>, IImmutable
    {
        public IEnumerable Items { get; }

        public ListReset(int fromVersion, IEnumerable items)
            :base(fromVersion)
        {
            Items = items;
        }

        #region Comparing

        public bool Equals(ListReset other)
        {
            return Equals(FromVersion, other.FromVersion)
                && Items.Cast<object>().SequenceEqual(other.Items.Cast<object>());
        }

        public override bool Equals(object other)
        {
            return (other as ListReset)?.Equals(this) == true;
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode(Items);
        }

        public static bool operator ==(ListReset x, ListReset y)
        {
            var isnull1 = ReferenceEquals(x, null);
            var isnull2 = ReferenceEquals(y, null);
            if (isnull1 && isnull2) return true;
            if (isnull1 || isnull2) return false;
            return x.Equals(y);
        }

        public static bool operator !=(ListReset x, ListReset y)
        {
            return !(x == y);
        }

        #endregion
    }

    public sealed class ListReset<T>: ListReset, IListChange<T>
    {
        public new ImmutableList<T> Items => (ImmutableList<T>)base.Items;

        public ListReset(int fromVersion, ImmutableList<T> items)
            :base(fromVersion, items)
        {}

        public static implicit operator ListReset<T>((int fromVersion, ImmutableList<T> items) value)
        {
            return new ListReset<T>(value.fromVersion, value.items);
        }
     

        public override string ToString()
        {
            var items = string.Join(", ", Items);
            return $"Ver {FromVersion} -> {FromVersion + 1}: Reset to: {items}";
        }
    }
}
