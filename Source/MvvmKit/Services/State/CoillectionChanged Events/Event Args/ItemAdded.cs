﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.CollectionChangeEvents
{
    public abstract class ItemAdded : Change, IItemAdded
    {
        public int Index { get; }

        public object Item { get; }

        public ItemAdded(int index, object item)
            :base(ChangeType.Added)
        {
            Item = item;
            Index = index;
        }

        #region Comparing
        public override bool Equals(object obj)
        {
            if (!(obj is ItemAdded)) return false;

            return this == (ItemAdded)obj;
        }

        public bool Equals(ItemAdded other)
        {
            return this == other;
        }

        public static bool operator ==(ItemAdded rs1, ItemAdded rs2)
        {
            var isnull1 = ReferenceEquals(rs1, null);
            var isnull2 = ReferenceEquals(rs2, null);

            if (isnull1 && isnull2) return true;
            if (isnull1 || isnull2) return false;

            return (rs1.Index == rs2.Index)
                && (rs1.Item == rs2.Item);
        }

        public static bool operator !=(ItemAdded rs1, ItemAdded rs2)
        {
            return !(rs1 == rs2);
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode(Item, Index);
        }

        #endregion

    }

    public class ItemAdded<T> : ItemAdded, IItemAdded<T>
    {
        public new T Item => (T)base.Item;

        public ItemAdded(int index, T item)
            :base(index, item)
        {
        }

        public static implicit operator ItemAdded<T>((int index, T item) value)
        {
            return new ItemAdded<T>(value.index, value.item);
        }

        public override string ToString()
        {
            return $"Added {Item} at {Index}";
        }

    }
}
