using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class StateCollectionProperty<K, T> : StateCollectionPropertyBase<K, T>
        where K: class
    {
        internal StateCollectionProperty(ServiceStore<K> store, ServiceBase owner, Expression<Func<K, IStateCollection<T>>> prop)
            :base(store, owner, prop)
        {
        }

        public Task Modify(Action<IStateCollection<T>> modifier)
        {
            return _runner.Run(() => _store.Modify(data => modifier(_getter(data))));
        }


    }
}
