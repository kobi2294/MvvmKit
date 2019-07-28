using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public interface IStateCollection<T> : IStateCollectionReader<T>
    {
        Task Set(IEnumerable<T> values);

        Task Modify(Action<IStateList<T>> modifier);
    }
}
