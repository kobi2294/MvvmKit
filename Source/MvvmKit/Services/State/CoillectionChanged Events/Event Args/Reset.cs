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
        public IEnumerable<object> Items { get; }

        public Reset(IEnumerable<object> items)
            : base(ChangeType.Reset)
        {
            Items = items;
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

            return (rs1.Items.Count() == rs2.Items.Count())
                && rs1.Items
                .Zip(rs2.Items, (a, b) => new { A = a, B = b })
                .All(pair => pair.A == pair.B);
        }

        public static bool operator !=(Reset rs1, Reset rs2)
        {
            return !(rs1 == rs2);
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GenerateHashCode(Items);
        }

        #endregion

    }

    public class Reset<T> : Reset, IReset<T>
    {
        public new IEnumerable<T> Items => base.Items.Cast<T>();

        public Reset(IEnumerable<T> items)
            : base(items.Cast<object>())
        {
        }

        public override string ToString()
        {
            var items = string.Join(", ", Items);
            return $"Reset to: {items}";
        }
    }
}
