using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class ListItemRemoved: ListChange, IEquatable<ListItemRemoved>, IImmutable
    {
        public int Index { get; }

        public object Item { get; }

        public ListItemRemoved(int fromVersion, int index, object item)
            :base(fromVersion)
        {
            Index = index;
            Item = item;
        }

        #region Comparing

        public bool Equals(ListItemRemoved other)
        {
            return Equals(FromVersion, other.FromVersion)
                && Equals(Index, other.Index)
                && Equals(Item, other.Item);
        }

        public override bool Equals(object other)
        {
            return (other as ListItemRemoved)?.Equals(this) == true;
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode(Item, Index);
        }

        public static bool operator ==(ListItemRemoved x, ListItemRemoved y)
        {
            var isnull1 = ReferenceEquals(x, null);
            var isnull2 = ReferenceEquals(y, null);
            if (isnull1 && isnull2) return true;
            if (isnull1 || isnull2) return false;
            return x.Equals(y);
        }

        public static bool operator !=(ListItemRemoved x, ListItemRemoved y)
        {
            return !(x == y);
        }

        #endregion

        #region Immutable

        public void Deconstruct(out int fromVersion, out int index, out object item)
        {
            fromVersion = FromVersion;
            index = Index;
            item = Item;
        }

        #endregion
    }

    public sealed class ListItemRemoved<T>: ListItemRemoved, IListChange<T>
    {
        public new T Item => (T)base.Item;

        public ListItemRemoved(int fromVersion, int index, T item)
            :base(fromVersion, index, item)
        {}

        public static implicit operator ListItemRemoved<T>((int fromVersion, int index, T item) value)
        {
            return new ListItemRemoved<T>(value.fromVersion, value.index, value.item);
        }

        public void Deconstruct(out int fromVersion, out int index, out T item)
        {
            fromVersion = FromVersion;
            index = Index;
            item = Item;
        }

        public override string ToString()
        {
            return $"Ver {FromVersion} -> {FromVersion + 1}: Removed {Item} at {Index}";
        }
    }
}
