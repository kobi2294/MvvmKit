using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.HeckelDiff
{
    public static class Operations
    {
        public abstract class Operation<T> : IEquatable<Operation<T>>
        {
            public abstract IEnumerable<int> GetIndices();

            public abstract IEnumerable<T> GetItems();

            public IEnumerable<object> Members
            {
                get
                {
                    yield return Name;
                    foreach (var item in GetIndices())
                    {
                        yield return item;
                    }
                    foreach (var item in GetItems())
                    {
                        yield return item;
                    }
                }
            }

            public string Name => GetType().Name;

            public override bool Equals(object obj)
            {
                return Equals(obj as Operation<T>);
            }

            public override int GetHashCode()
            {
                return ObjectExtensions.GenerateHashCode(Members);
            }

            public override string ToString()
            {
                return string.Join(" ", Members);
            }

            public bool Equals(Operation<T> obj)
            {
                return (obj is Operation<T> op)
                    && (obj.GetType() == GetType())
                    && (op.Members.SequenceEqual(Members));
            }

            public static bool operator ==(Operation<T> o1, Operation<T> o2)
            {
                var isnull1 = ReferenceEquals(o1, null);
                var isnull2 = ReferenceEquals(o2, null);

                if (isnull1 && isnull2) return true;
                if (isnull1 || isnull2) return false;

                return o1.Equals(o2);
            }

            public static bool operator !=(Operation<T> o1, Operation<T> o2)
            {
                return !(o1 == o2);
            }

        }

        public static InsertOp<T> Insert<T>(int index, T item) => new InsertOp<T>(index, item);

        public static DeleteOp<T> Delete<T>(int index, T item) => new DeleteOp<T>(index, item);

        public static UpdateOp<T> Update<T>(int index, T fromItem, T toItem) => new UpdateOp<T>(index, fromItem, toItem);

        public static MoveOp<T> Move<T>(int from, int to, T item) => new MoveOp<T>(from, to, item);


        public class InsertOp<T> : Operation<T>
        {
            public override IEnumerable<int> GetIndices()
            {
                yield return Index;
            }

            public override IEnumerable<T> GetItems()
            {
                yield return Item;
            }

            public int Index { get; }

            public T Item { get; }

            public InsertOp(int index, T item)
            {
                Index = index;
                Item = item;
            }
        }

        public class DeleteOp<T> : Operation<T>
        {
            public override IEnumerable<int> GetIndices()
            {
                yield return Index;
            }

            public override IEnumerable<T> GetItems()
            {
                yield return Item;
            }

            public int Index { get; }

            public T Item { get; }

            public DeleteOp(int index, T item)
            {
                Index = index;
                Item = item;
            }
        }

        public class UpdateOp<T> : Operation<T>
        {
            public override IEnumerable<int> GetIndices()
            {
                yield return Index;
            }

            public override IEnumerable<T> GetItems()
            {
                yield return FromItem;
                yield return ToItem;
            }

            public T FromItem { get; }

            public T ToItem { get; }

            public int Index { get; }

            public UpdateOp(int index, T fromItem, T toItem)
            {
                Index = index;
                FromItem = fromItem;
                ToItem = toItem;
            }
        }

        public class MoveOp<T> : Operation<T>
        {
            public override IEnumerable<int> GetIndices()
            {
                yield return From;
                yield return To;
            }

            public override IEnumerable<T> GetItems()
            {
                yield return Item;
            }

            public T Item { get; }
            public int From { get; }
            public int To { get; }
            public MoveOp(int from, int to, T item)
            {
                From = from;
                To = to;
                Item = item;
            }
        }
    }
}
