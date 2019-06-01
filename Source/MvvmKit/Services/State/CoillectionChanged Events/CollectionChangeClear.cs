using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class CollectionChangeClear : ICollectionChangeClear
    {
        public CollectionChangeClear()
        {
        }

        #region Comparing
        public override bool Equals(object obj)
        {
            if (!(obj is CollectionChangeClear)) return false;

            return this == (CollectionChangeClear)obj;
        }

        public bool Equals(CollectionChangeClear other)
        {
            return this == other;
        }

        public static bool operator ==(CollectionChangeClear rs1, CollectionChangeClear rs2)
        {
            var isnull1 = ReferenceEquals(rs1, null);
            var isnull2 = ReferenceEquals(rs2, null);

            if (isnull1 && isnull2) return true;
            if (isnull1 || isnull2) return false;

            return true;
        }

        public static bool operator !=(CollectionChangeClear rs1, CollectionChangeClear rs2)
        {
            return !(rs1 == rs2);
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode();
        }

        #endregion

    }

    public class CollectionChangeClear<T> : CollectionChangeClear, ICollectionChangeClear<T>
    {
        public CollectionChangeClear()
            : base()
        {
        }
    }
}
