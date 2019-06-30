using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.CollectionChangeEvents
{
    public abstract class Reset : Change, IReset
    {
        public Reset(IEnumerable currentItems)
            :base(ChangeType.Reset, currentItems)
        {
        }

        #region Comparing
        public override bool Equals(object obj)
        {
            if (!(obj is Reset)) return false;

            return this == (Reset)obj;
        }

        public bool Equals(Reset other)
        {
            return this == other;
        }

        public static bool operator ==(Reset rs1, Reset rs2)
        {
            var isnull1 = ReferenceEquals(rs1, null);
            var isnull2 = ReferenceEquals(rs2, null);

            if (isnull1 && isnull2) return true;
            if (isnull1 || isnull2) return false;

            return rs1.CurrentItems.SequenceEqual(rs2.CurrentItems);
        }

        public static bool operator !=(Reset rs1, Reset rs2)
        {
            return !(rs1 == rs2);
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode(CurrentItems);
        }

        #endregion

    }

    public class Reset<T> : Reset, IReset<T>
    {
        private IReadOnlyList<T> _currentItems;
        IReadOnlyList<T> IChange<T>.CurrentItems => _currentItems;

        public Reset(IEnumerable<T> items)
            :base(items)
        {
            _currentItems = GetCurrentItems<T>();
        }


        public override string ToString()
        {
            var items = string.Join(", ", CurrentItems);
            return $"Reset to: {items}";
        }
    }
}
