using Castle.DynamicProxy;
using MvvmKit.CollectionChangeEvents;
using System;
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

        public AccessInterceptor(InterfaceState state, bool allowModifications = false)
        {
            _state = state;
            _allowModifications = allowModifications;
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
            if (prop.PropertyType.IsGenericOf(typeof(IStateCollection<>)))
            {
                // if the property is of type state collection, we need a special treatment becuase the stored 
                // data is of type List<T>, and also, because we need to monitor changes.

                var itemType = prop.PropertyType.GenericTypeArguments[0];
                var proxyType = typeof(CollectionProxy<>).MakeGenericType(itemType);

                var list = _state[prop];
                Action<IChange> onChange = change => _onChange(prop, change);
                var proxy = Activator.CreateInstance(proxyType, list, this, onChange, _allowModifications);
                invocation.ReturnValue = proxy;
            } else
            {
                invocation.ReturnValue = _state[prop];
            }
        }

        private void _onChange(PropertyInfo prop, IChange change)
        {
            Console.WriteLine($"property: {prop.Name}, {change}");
        }

        private void _doSet(PropertyInfo prop, IInvocation invocation)
        {
            if (!_allowModifications)
                throw new InvalidOperationException($"Attempting to set a property during a read only operation: {prop.Name}");
            _state[prop] = invocation.Arguments[0];
        }
    }
}
