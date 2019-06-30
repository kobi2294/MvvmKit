using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.CollectionChangeEvents
{
    public abstract class ItemMoved : Change, IItemMoved
    {
        public int FromIndex { get; }

        public int ToIndex { get; }

        public object Item { get; }

        public ItemMoved(int fromIndex, int toIndex, object item, IEnumerable currentItems)
            :base(ChangeType.Moved, currentItems)
        {
            Item = item;
            FromIndex = fromIndex;
            ToIndex = toIndex;
        }

        #region Comparing
        public override bool Equals(object obj)
        {
            if (!(obj is ItemMoved)) return false;

            return this == (ItemMoved)obj;
        }

        public bool Equals(ItemMoved other)
        {
            return this == other;
        }

        public static bool operator ==(ItemMoved rs1, ItemMoved rs2)
        {
            var isnull1 = ReferenceEquals(rs1, null);
            var isnull2 = ReferenceEquals(rs2, null);

            if (isnull1 && isnull2) return true;
            if (isnull1 || isnull2) return false;

            return (rs1.FromIndex == rs2.FromIndex)
                && (rs1.ToIndex == rs2.ToIndex)
                && (rs1.Item == rs2.Item)
                && rs1.CurrentItems.SequenceEqual(rs2.CurrentItems);
        }

        public static bool operator !=(ItemMoved rs1, ItemMoved rs2)
        {
            return !(rs1 == rs2);
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode(Item, FromIndex, ToIndex, ObjectExtensions.GenerateHashCode(CurrentItems));
        }

        #endregion

    }

    public class ItemMoved<T> : ItemMoved, IItemMoved<T>
    {
        public new T Item => (T)base.Item;

        private IReadOnlyList<T> _currentItems;
        IReadOnlyList<T> IChange<T>.CurrentItems => _currentItems;

        public ItemMoved(int fromIndex, int toIndex, T item, IEnumerable<T> currentItems)
            : base(fromIndex, toIndex, item, currentItems)
        {
            _currentItems = GetCurrentItems<T>();
        }

        public static implicit operator ItemMoved<T>((int fromIndex, int toIndex, T item, IEnumerable<T> currentItems) value)
        {
            return new ItemMoved<T>(value.fromIndex, value.toIndex, value.item, value.currentItems);
        }

        public override string ToString()
        {
            return $"Moved {Item} from {FromIndex} to {ToIndex}";
        }

    }
}
