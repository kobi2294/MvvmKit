using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public ServiceStore()
        {
            var type = typeof(T);

            if (!type.IsInterface)
                throw new ArgumentException("Service Store only supports interface types", nameof(T));

            _state = new InterfaceState(type);
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
            }
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

        public AsyncEvent<K> Observe<K>(Expression<Func<T, K>> property)
        {
            return null;
        }
    }
}
