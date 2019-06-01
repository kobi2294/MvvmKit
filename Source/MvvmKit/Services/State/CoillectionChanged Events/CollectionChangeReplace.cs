using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class CollectionChangeReplace : ICollectionChangeReplace
    {
        public int Index { get; }

        public object FromItem { get; }

        public object ToItem { get; }

        public CollectionChangeReplace(int index, object fromItem, object toItem)
        {
            FromItem = fromItem;
            ToItem = toItem;
            Index = index;
        }

        #region Comparing
        public override bool Equals(object obj)
        {
            if (!(obj is CollectionChangeReplace)) return false;

            return this == (CollectionChangeReplace)obj;
        }

        public bool Equals(CollectionChangeReplace other)
        {
            return this == other;
        }

        public static bool operator ==(CollectionChangeReplace rs1, CollectionChangeReplace rs2)
        {
            var isnull1 = ReferenceEquals(rs1, null);
            var isnull2 = ReferenceEquals(rs2, null);

            if (isnull1 && isnull2) return true;
            if (isnull1 || isnull2) return false;

            return ((rs1.Index == rs2.Index) && (rs1.FromItem == rs2.FromItem) && (rs1.ToItem == rs2.ToItem));
        }

        public static bool operator !=(CollectionChangeReplace rs1, CollectionChangeReplace rs2)
        {
            return !(rs1 == rs2);
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode(FromItem, ToItem, Index);
        }

        #endregion

    }

    public class CollectionChangeReplace<T> : CollectionChangeReplace, ICollectionChangeReplace<T>
    {
        public new T FromItem => (T)base.FromItem;

        public new T ToItem => (T)base.ToItem;

        public CollectionChangeReplace(int index, T fromItem, T toItem)
            : base(index, fromItem, toItem)
        {
        }

        public static implicit operator CollectionChangeReplace<T>((int index, T fromItem, T toItem) value)
        {
            return new CollectionChangeReplace<T>(value.index, value.fromItem, value.toItem);
        }

    }
}
