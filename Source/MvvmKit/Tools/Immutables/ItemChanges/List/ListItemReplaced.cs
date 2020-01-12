using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class ListItemReplaced: ListChange, IEquatable<ListItemReplaced>, IImmutable
    {
        public int Index { get; }

        public object OldItem { get; }

        public object NewItem { get; }

        public ListItemReplaced(int fromVersion, int index, object oldItem, object newItem)
            :base(fromVersion)
        {
            Index = index;
            OldItem = oldItem;
            NewItem = newItem;
        }

        #region Comparing

        public bool Equals(ListItemReplaced other)
        {
            return Equals(FromVersion, other.FromVersion)
                && Equals(Index, other.Index)
                && Equals(OldItem, other.OldItem)
                && Equals(NewItem, other.NewItem);                
        }

        public override bool Equals(object other)
        {
            return (other as ListItemReplaced)?.Equals(this) == true;
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode(Index, OldItem, NewItem);
        }

        public static bool operator ==(ListItemReplaced x, ListItemReplaced y)
        {
            var isnull1 = ReferenceEquals(x, null);
            var isnull2 = ReferenceEquals(y, null);
            if (isnull1 && isnull2) return true;
            if (isnull1 || isnull2) return false;
            return x.Equals(y);
        }

        public static bool operator !=(ListItemReplaced x, ListItemReplaced y)
        {
            return !(x == y);
        }

        #endregion

        #region Immutable

        public void Deconstruct(out int fromVersion, out int index, out object oldItem, out object newItem)
        {
            fromVersion = FromVersion;
            index = Index;
            oldItem = OldItem;
            newItem = NewItem;
        }

        #endregion
    }

    public sealed class ListItemReplaced<T>: ListItemReplaced, IListChange<T>
    {
        public new T OldItem => (T)base.OldItem;
        public new T NewItem => (T)base.NewItem;

        public ListItemReplaced(int fromVersion, int index, T oldItem, T newItem)
            :base(fromVersion, index, oldItem, newItem)
        {}

        public static implicit operator ListItemReplaced<T>((int fromVersion, int index, T oldItem, T newItem) value)
        {
            return new ListItemReplaced<T>(value.fromVersion, value.index, value.oldItem, value.newItem);
        }

        public void Deconstruct(out int fromVersion, out int index, out T oldItem, out T newItem)
        {
            fromVersion = FromVersion;
            index = Index;
            oldItem = OldItem;
            newItem = NewItem;
        }

        public override string ToString()
        {
            return $"Ver {FromVersion} -> {FromVersion + 1}: Replaced on {Index} from {OldItem} to {NewItem}";
        }
    }
}
