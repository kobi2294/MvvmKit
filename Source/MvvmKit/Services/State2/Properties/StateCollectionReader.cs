using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class StateCollectionReader<K, T>: IStateCollectionReader<T>
        where K: class
    {
        protected ServiceStore<K> _store;
        protected ServiceBase _owner;
        protected ServiceBase.Runner _runner;
        protected Func<K, IStateList<T>> _getter;

        internal StateCollectionReader(ServiceStore<K> store, ServiceBase owner, Expression<Func<K, IStateList<T>>> prop)
        {
            _store = store;
            _owner = owner;
            _runner = _owner.GetRunner();
            _getter = prop.Compile();

            Changed = _store.Observe(prop);
        }
        public AsyncEvent<CollectionChanges<T>> Changed { get; }

        public async Task<IReadOnlyList<T>> Get()
        {
            var data = await Select(x => x.ToReadOnly());
            return data;
        }

        public Task<TRes> Select<TRes>(Func<IStateList<T>, TRes> selector)
        {
            return _runner.Run(() => _store.Select(data => selector(_getter(data))));
        }
    }
}
