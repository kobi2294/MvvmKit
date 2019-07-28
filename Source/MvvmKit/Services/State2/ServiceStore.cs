using Castle.DynamicProxy;
using MvvmKit.CollectionChangeEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ServiceStore<T>
        where T: class
    {
        private InterfaceState _state;
        private AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();
        private ProxyGenerator _proxyGenerator = new ProxyGenerator();
        private Dictionary<PropertyInfo, IAsyncEventWithData> _events;

        public ServiceStore()
        {
            var type = typeof(T);

            if (!type.IsInterface)
                throw new ArgumentException("Service Store only supports interface types", nameof(T));

            _state = new InterfaceState(type);
            _events = new Dictionary<PropertyInfo, IAsyncEventWithData>();
        }

        public ServiceStore(Action<T> init)
            :this()
        {
            using (var inter = new AccessInterceptor(_state, true))
            {
                var proxy = _createProxy(inter);
                init(proxy);
            }
        }

        private T _createProxy(AccessInterceptor interceptor)
        {
            var proxy = _proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(interceptor);
            return proxy;
        }

        public async Task Modify(Action<T> modifer)
        {
            using (var inter = new AccessInterceptor(_state, true))
            {
                var proxy = _createProxy(inter);
                using (await _lock.WriterLock())
                {
                    modifer(proxy);
                }

                // now that the lock is released, it's time to raise the proper events
                var literalEventTasks = inter.ChangedLiterals()
                    .Where(pair => _events.ContainsKey(pair.prop))
                    .Select(pair => _events[pair.prop].Invoke(pair.value));

                var collectionEventTasks = inter.ChangedCollections()
                    .Where(item => _events.ContainsKey(item.Key))
                    .Select(item => _events[item.Key].Invoke(
                        _createCollectionChangesArg(item, inter.OldCollectionValue(item.Key))));

                await Task.WhenAll(literalEventTasks.Concat(collectionEventTasks));
            }
        }

        private object _createCollectionChangesArg(IGrouping<PropertyInfo, IChange> group, object[] oldVals)
        {
            var prop = group.Key;
            var stateCollectionType = prop.PropertyType;
            // we assume stateCollectionType is IStateCollection<T>
            var itemType = stateCollectionType.GenericTypeArguments[0];

            var argsType = typeof(CollectionChanges<>).MakeGenericType(itemType);
            var changes = group.ToArray();
            var newVals = (IEnumerable)_state[prop];
            var instance = Activator.CreateInstance(argsType, new object[] { changes, (IEnumerable)oldVals, newVals });
            return instance;
        }

        public async Task<K> Select<K>(Func<T, K> query)
        {
            K res = default;

            using (var inter = new AccessInterceptor(_state, false))
            {
                var proxy = _createProxy(inter);
                using (await _lock.ReaderLock())
                {
                    res = query(proxy);
                }
            }
            return res;
        }

        public AsyncEvent<CollectionChanges<K>> Observe<K>(Expression<Func<T, IStateCollection<K>>> property)
        {
            var prop = property.GetProperty();
            if (_events.ContainsKey(prop))
                return _events[prop] as AsyncEvent<CollectionChanges<K>>;

            var val = (List<K>)_state[prop];
            var ae = new AsyncEvent<CollectionChanges<K>>(Changes.Init(val))
                .OnSubscribe(async cb => await cb.Invoke(Changes.Init((List<K>)_state[prop])));
            _events.Add(prop, ae);
            return ae;
        }

        public AsyncEvent<K> Observe<K>(Expression<Func<T, K>> property)
        {
            var prop = property.GetProperty();
            if (_events.ContainsKey(prop))
                return _events[prop] as AsyncEvent<K>;

            var val = (K)_state[prop];
            var ae = new AsyncEvent<K>(val);
            _events.Add(prop, ae);
            return ae;
        }

        public StateCollectionPropertyBase<T, K> CreateReader<K>(ServiceBase service, Expression<Func<T, IStateCollection<K>>> prop)
        {
            var res = new StateCollectionPropertyBase<T, K>(this, service, prop);
            return res;
        }

        public StateCollectionProperty<T, K> CreateWriter<K>(ServiceBase service, Expression<Func<T, IStateCollection<K>>> prop)
        {
            var res = new StateCollectionProperty<T, K>(this, service, prop);
            return res;
        }

        public StatePropertyBase<T, K> CreateReader<K>(ServiceBase service, Expression<Func<T, K>> prop)
        {
            var res = new StatePropertyBase<T, K>(this, service, prop);
            return res;
        }

        public StateProperty<T, K> CreateWriter<K>(ServiceBase service, Expression<Func<T, K>> prop)
        {
            var res = new StateProperty<T, K>(this, service, prop);
            return res;
        }
    }
}
