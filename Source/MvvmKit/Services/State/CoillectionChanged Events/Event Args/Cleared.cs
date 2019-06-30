using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.CollectionChangeEvents
{
    public abstract class Cleared : Change, ICleared
    {
        public Cleared()
            :base(ChangeType.Cleared, Enumerable.Empty<object>())
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
        private IReadOnlyList<T> _currentItems;
        IReadOnlyList<T> IChange<T>.CurrentItems => _currentItems;

        public Cleared()
            : base()
        {
            _currentItems = GetCurrentItems<T>();
        }


        public override string ToString()
        {
            return "Cleared";
        }
    }
}
