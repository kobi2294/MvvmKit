using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.CollectionChangeEvents
{
    public abstract class Cleared : Change, ICleared
    {
        public Cleared()
            :base(ChangeType.Cleared)
        {
        }

        #region Comparing
        public override bool Equals(object obj)
        {
            if (!(obj is Cleared)) return false;

            return this == (Cleared)obj;
        }

        public bool Equals(Cleared other)
        {
            return this == other;
        }

        public static bool operator ==(Cleared rs1, Cleared rs2)
        {
            var isnull1 = ReferenceEquals(rs1, null);
            var isnull2 = ReferenceEquals(rs2, null);

            if (isnull1 && isnull2) return true;
            if (isnull1 || isnull2) return false;

            return true;
        }

        public static bool operator !=(Cleared rs1, Cleared rs2)
        {
            return !(rs1 == rs2);
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode();
        }

        #endregion

    }

    public class Cleared<T> : Cleared, ICleared<T>
    {
        public Cleared()
            : base()
        {
        }

        public override string ToString()
        {
            return "Cleared";
        }
    }
}
