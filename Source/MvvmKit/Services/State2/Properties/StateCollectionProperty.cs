using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class StateCollectionProperty<K, T> : StateCollectionReader<K, T>, IStateCollectionProperty<T>
        where K: class
    {
        internal StateCollectionProperty(ServiceStore<K> store, ServiceBase owner, Expression<Func<K, IStateList<T>>> prop)
            :base(store, owner, prop)
        {
        }

        public Task Modify(Action<IStateList<T>> modifier)
        {
            return _runner.Run(() => _store.Modify(data => modifier(_getter(data))));
        }

        public Task Set(IEnumerable<T> values)
        {
            return Modify(collection => collection.Reset(values));
        }
    }
}
