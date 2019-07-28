using Castle.DynamicProxy;
using MvvmKit.CollectionChangeEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class AccessInterceptor : BaseDisposable, IInterceptor
    {
        private InterfaceState _state;
        private bool _allowModifications;

        private Dictionary<PropertyInfo, object> _oldValues;
        private Dictionary<PropertyInfo, object[]> _oldCollectionValues;
        private EditableLookup<PropertyInfo, IChange> _collectionChanges;
        private Dictionary<PropertyInfo, object> _literalChanges;
        private Dictionary<PropertyInfo, IStateList> _proxies;


        public AccessInterceptor(InterfaceState state, bool allowModifications = false)
        {
            _state = state;
            _allowModifications = allowModifications;

            _oldValues = state.Properties.ToDictionary(p => p, p => state[p]);
            _oldCollectionValues = state.CollectionProperties
                .ToDictionary(p => p, p => (state[p] as IEnumerable).Cast<object>().ToArray());
            _collectionChanges = new EditableLookup<PropertyInfo, IChange>();
            _literalChanges = new Dictionary<PropertyInfo, object>();
            _proxies = new Dictionary<PropertyInfo, IStateList>();
        }

        public IEnumerable<(PropertyInfo prop, object value)> ChangedLiterals()
        {
            return _literalChanges.Select(pair => (pair.Key, pair.Value));
        }

        public IEnumerable<IGrouping<PropertyInfo, IChange>> ChangedCollections()
        {
            return _collectionChanges;
        }

        public object[] OldCollectionValue(PropertyInfo prop)
        {
            return _oldCollectionValues[prop];
        }

        private IStateList _ensureProxy(PropertyInfo prop)
        {
            if (_proxies.ContainsKey(prop)) return _proxies[prop];
            var itemType = prop.PropertyType.GenericTypeArguments[0];
            var proxyType = typeof(CollectionProxy<>).MakeGenericType(itemType);

            var list = _state[prop];
            Action<IChange> onChange = change => _onChange(prop, change);
            var proxy = Activator.CreateInstance(proxyType, list, this, onChange, _allowModifications) as IStateList;
            _proxies.Add(prop, proxy);
            return proxy;

        }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.Method;
            var member = _state.PropertyOf(method);

            switch (member.role)
            {
                case PropertyMethodRole.None:
                    throw new InvalidOperationException($"Attempting to invoke a state member that is not a property: {method.Name}");
                case PropertyMethodRole.Getter:
                    _doGet(member.prop, invocation);
                    break;
                case PropertyMethodRole.Setter:
                    _doSet(member.prop, invocation);
                    break;
            }
        }

        private void _doGet(PropertyInfo prop, IInvocation invocation)
        {
            if (_state.IsCollection(prop))
            {
                // if the property is of type state collection, we need a special treatment becuase the stored 
                // data is of type List<T>, and also, because we need to monitor changes.
                var proxy = _ensureProxy(prop);
                invocation.ReturnValue = proxy;
            } else
            {
                invocation.ReturnValue = _state[prop];
            }
        }

        private void _onChange(PropertyInfo prop, IChange change)
        {
            _collectionChanges.Add(prop, change);
        }

        private void _doSet(PropertyInfo prop, IInvocation invocation)
        {
            if (!_allowModifications)
                throw new InvalidOperationException($"Attempting to set a property during a read only operation: {prop.Name}");
            _state[prop] = invocation.Arguments[0];
            _literalChanges[prop] = invocation.Arguments[0];
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
        }
    }
}
