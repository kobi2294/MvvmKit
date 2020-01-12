using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class ListItemMoved: ListChange, IEquatable<ListItemMoved>, IImmutable
    {
        public int OldIndex { get; }

        public int NewIndex { get; }

        public object Item { get; }

        public ListItemMoved(int fromVersion, int oldIndex, int newIndex, object item)
            :base(fromVersion)
        {
            OldIndex = oldIndex;
            NewIndex = newIndex;
            Item = item;
        }

        #region Comparing

        public bool Equals(ListItemMoved other)
        {
            return Equals(FromVersion, other.FromVersion)
                && Equals(OldIndex, other.OldIndex)
                && Equals(NewIndex, other.NewIndex)
                && Equals(Item, other.Item);                
        }

        public override bool Equals(object other)
        {
            return (other as ListItemMoved)?.Equals(this) == true;
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode(Item, OldIndex);
        }

        public static bool operator ==(ListItemMoved x, ListItemMoved y)
        {
            var isnull1 = ReferenceEquals(x, null);
            var isnull2 = ReferenceEquals(y, null);
            if (isnull1 && isnull2) return true;
            if (isnull1 || isnull2) return false;
            return x.Equals(y);
        }

        public static bool operator !=(ListItemMoved x, ListItemMoved y)
        {
            return !(x == y);
        }

        #endregion

        #region Immutable

        public void Deconstruct(out int fromVersion, out int oldIndex, out int newIndex, out object item)
        {
            fromVersion = FromVersion;
            oldIndex = OldIndex;
            newIndex = NewIndex;
            item = Item;
        }

        #endregion
    }

    public sealed class ListItemMoved<T>: ListItemMoved, IListChange<T>
    {
        public new T Item => (T)base.Item;

        public ListItemMoved(int fromVersion, int oldIndex, int newIndex, T item)
            :base(fromVersion, oldIndex, newIndex, item)
        {}

        public static implicit operator ListItemMoved<T>((int fromVersion, int oldIndex, int newIndex, T item) value)
        {
            return new ListItemMoved<T>(value.fromVersion, value.oldIndex, value.newIndex, value.item);
        }

        public void Deconstruct(out int fromVersion, out int oldIndex, out int newIndex, out T item)
        {
            fromVersion = FromVersion;
            oldIndex = OldIndex;
            newIndex = NewIndex;
            item = Item;
        }

        public override string ToString()
        {
            return $"Ver {FromVersion} -> {FromVersion + 1}: Moved {Item} from {OldIndex} to {NewIndex}";
        }
    }
}
