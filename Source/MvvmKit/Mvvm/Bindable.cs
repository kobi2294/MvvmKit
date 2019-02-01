using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class Bindable : BindableBase
    {
        private LazyDictionary<string, PropertyChangeListener> _listeners;
       
        private void _callListeners<T>(string propertyName, T oldval, T newval)
        {
            Properties[propertyName].Invoke(oldval, newval);
        }

        IReadOnlyIndexer<string, Expression<Func<object>>, PropertyChangeListener> _properties;
        public IReadOnlyIndexer<string, Expression<Func<object>>, PropertyChangeListener> Properties
        {
            get
            {
                if (_properties == null)
                {
                    _listeners = new LazyDictionary<string, PropertyChangeListener>(propName => new PropertyChangeListener());
                    _properties = Indexers
                        .ReadOnly(_listeners)
                        .And((Expression<Func<object>> exp) => Properties[exp.GetName()]);
                }
                return _properties;
            }
        }

        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            var oldVal = storage;
            var wasChanged = base.SetProperty(ref storage, value, propertyName);

            if (wasChanged)
            {
                _callListeners<T>(propertyName, oldVal, value);
            }

            return wasChanged;
        }

        public Bindable()
        {
        }
    }
}
