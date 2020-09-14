using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class StateProperty<K, T> : StatePropertyReader<K, T>, IStateProperty<T>
        where K: class
    {
        Action<K, T> _setter;

        internal StateProperty(ServiceStore<K> store, ServiceBase owner, Expression<Func<K, T>> prop)
            :base(store, owner, prop)
        {
            _setter = prop.ToSetter();
        }

        public Task Set(T value)
        {
            return _runner.Run(() => _store.Modify((data) => _setter(data, value)));
        }

    }
}
