using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class StatePropertyBase<K, T>
        where K: class
    {
        protected ServiceStore<K> _store;
        protected ServiceBase.Runner _runner;
        protected ServiceBase _owner;
        protected Func<K, T> _getter;

        internal StatePropertyBase(ServiceStore<K> store, ServiceBase owner, Expression<Func<K, T>> prop)
        {
            _store = store;
            _owner = owner;
            _runner = owner.GetRunner();

            Changed = _store.Observe(prop);
            _getter = prop.Compile();
        }

        public AsyncEvent<T> Changed { get; }

        public Task<T> Get()
        {
            return _runner.Run(() => _store.Select(_getter));
        }
    }
}
