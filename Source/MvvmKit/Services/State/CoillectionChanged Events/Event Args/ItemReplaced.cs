using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.CollectionChangeEvents
{
    public abstract class ItemReplaced : Change, IItemReplaced
    {
        public int Index { get; }

        public object FromItem { get; }

        public object ToItem { get; }

        public ItemReplaced(int index, object fromItem, object toItem)
            :base(ChangeType.Replaced)
        {
            FromItem = fromItem;
            ToItem = toItem;
            Index = index;
        }

        #region Comparing
        public override bool Equals(object obj)
        {
            if (!(obj is ItemReplaced)) return false;

            return this == (ItemReplaced)obj;
        }

        public bool Equals(ItemReplaced other)
        {
            return this == other;
        }

        public static bool operator ==(ItemReplaced rs1, ItemReplaced rs2)
        {
            var isnull1 = ReferenceEquals(rs1, null);
            var isnull2 = ReferenceEquals(rs2, null);

            if (isnull1 && isnull2) return true;
            if (isnull1 || isnull2) return false;

            return ((rs1.Index == rs2.Index) && (rs1.FromItem == rs2.FromItem) && (rs1.ToItem == rs2.ToItem));
        }

        public static bool operator !=(ItemReplaced rs1, ItemReplaced rs2)
        {
            return !(rs1 == rs2);
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode(FromItem, ToItem, Index);
        }

        #endregion

    }

    public class ItemReplaced<T> : ItemReplaced, IItemReplaced<T>
    {
        public new T FromItem => (T)base.FromItem;

        public new T ToItem => (T)base.ToItem;

        public ItemReplaced(int index, T fromItem, T toItem)
            : base(index, fromItem, toItem)
        {
        }

        public static implicit operator ItemReplaced<T>((int index, T fromItem, T toItem) value)
        {
            return new ItemReplaced<T>(value.index, value.fromItem, value.toItem);
        }

        public override string ToString()
        {
            return $"Replaced at {Index} from {FromItem} to {ToItem}";
        }

    }
}
