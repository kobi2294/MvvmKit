using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public interface IStateCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection, IReadOnlyList<T>, IReadOnlyCollection<T>
    {
        bool Exists(Predicate<T> predicate);

        T Find(Predicate<T> predicate);

        int FindIndex(Predicate<T> predicate);

        void SetWhere(Predicate<T> predicate, T item);

        void AddRange(IEnumerable<T> values);

        void AddRange(params T[] values);

        void InsertRange(int index, IEnumerable<T> values);

        void InsertRange(int index, params T[] values);

        void MoveAt(int oldIndex, int newIndex);

        void MoveItem(T item, int newIndex);

        void RemoveWhere(Predicate<T> predicate);

        void Reset(IEnumerable<T> values);

        void Reset(params T[] values);
    }
}
