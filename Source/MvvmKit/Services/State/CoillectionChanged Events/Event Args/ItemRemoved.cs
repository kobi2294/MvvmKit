﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.CollectionChangeEvents
{
    public abstract class ItemRemoved : Change, IItemRemoved
    {
        public int Index { get; }

        public object Item { get; }

        public ItemRemoved(int index, object item)
            :base(ChangeType.Removed)
        {
            Item = item;
            Index = index;
        }

        #region Comparing
        public override bool Equals(object obj)
        {
            if (!(obj is ItemRemoved)) return false;

            return this == (ItemRemoved)obj;
        }

        public bool Equals(ItemRemoved other)
        {
            return this == other;
        }

        public static bool operator ==(ItemRemoved rs1, ItemRemoved rs2)
        {
            var isnull1 = ReferenceEquals(rs1, null);
            var isnull2 = ReferenceEquals(rs2, null);

            if (isnull1 && isnull2) return true;
            if (isnull1 || isnull2) return false;

            return (rs1.Index == rs2.Index)
                && (rs1.Item == rs2.Item);
        }

        public static bool operator !=(ItemRemoved rs1, ItemRemoved rs2)
        {
            return !(rs1 == rs2);
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode(Item, Index);
        }

        #endregion

    }

    public class ItemRemoved<T> : ItemRemoved, IItemRemoved<T>
    {
        public new T Item => (T)base.Item;

        public ItemRemoved(int index, T item)
            : base(index, item)
        {
        }

        public static implicit operator ItemRemoved<T>((int index, T item) value)
        {
            return new ItemRemoved<T>(value.index, value.item);
        }

        public override string ToString()
        {
            return $"Removed {Item} from {Index}";
        }
    }
}
