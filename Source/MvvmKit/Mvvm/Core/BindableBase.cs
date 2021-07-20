using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class BindableBase : PrismBindableBase, INotifyDisposable
    {
        #region INotifyDisposable

        private INotifyDisposable _handler = MvvmKit.Disposables.Notifier();

        public bool IsDisposed => _handler.IsDisposed;

        public void Attach(IDisposable child, bool keepAlive = false) => _handler.Attach(child, keepAlive);

        public void Dettach(IDisposable child) => _handler.Dettach(child);

        public void AttachMany<T>(IEnumerable<T> children, bool keepAlive = false)
            where T : IDisposable
        {
            _handler.AttachMany(children, keepAlive);
        }

        public void DettachMany<T>(IEnumerable<T> children)
            where T : IDisposable
        {
            _handler.DettachMany(children);
        }

        public void Validate()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        protected virtual void OnDisposed()
        {
            if (_listeners != null)
            {
                foreach (var listener in _listeners.Values)
                {
                    listener.Dispose();
                }
                _listeners = null;
            }
        }

        public void DisposeIfNeeded()
        {
            if (!IsDisposed)
                Dispose();
        }

        public void Dispose()
        {
            if (IsDisposed) return;
            _handler.Dispose();
            OnDisposed();
        }

        #endregion

        private Dictionary<string, PropertyChangeListener> _listeners;

        private void _callListenersIfExist<T>(string propertyName, T oldval, T newval)
        {
            if (_listeners == null) return;
            if (_listeners.ContainsKey(propertyName))
            {
                _listeners[propertyName].Invoke(oldval, newval);
            }
        }

        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            var oldVal = storage;
            var wasChanged = base.SetProperty(ref storage, value, propertyName);

            if (wasChanged)
            {
                _callListenersIfExist<T>(propertyName, oldVal, value);
            }

            return wasChanged;
        }

        private PropertyChangeListener<T> _ensureListener<T>(string propertyName)
        {
            if (_listeners == null) _listeners = new Dictionary<string, PropertyChangeListener>();
            if (!(_listeners.ContainsKey(propertyName)))
            {
                _listeners.Add(propertyName, new PropertyChangeListener<T>());
            }

            return _listeners[propertyName] as PropertyChangeListener<T>;
        }

        private bool _hasListener(string propertyName)
        {
            if (_listeners == null) return false;
            return _listeners.ContainsKey(propertyName);
        }

        public BindableBase()
        {
        }

        #region All Versions of Observe

        public void Observe<T>(string propertyName, object owner, Action action)
        {
            var listner = _ensureListener<T>(propertyName);
            listner.Observe(owner, action);
        }

        public void Observe<T>(string propertyName, Action action)
        {
            Observe<T>(propertyName, this, action);
        }

        public void Observe<T>(Expression<Func<T>> property, object owner, Action action)
        {
            Observe<T>(property.GetName(), owner, action);
        }

        public void Observe<T>(Expression<Func<T>> property, Action action)
        {
            Observe(property, this, action);
        }

        public void Observe<T>(string propertyName, object owner, Action<T> action)
        {
            var listner = _ensureListener<T>(propertyName);
            listner.Observe(owner, action);
        }

        public void Observe<T>(string propertyName, Action<T> action)
        {
            Observe<T>(propertyName, this, action);
        }

        public void Observe<T>(Expression<Func<T>> property, object owner, Action<T> action)
        {
            Observe<T>(property.GetName(), owner, action);
        }

        public void Observe<T>(Expression<Func<T>> property, Action<T> action)
        {
            Observe(property, this, action);
        }

        public void Observe<T>(string propertyName, object owner, Action<T, T> action)
        {
            var listner = _ensureListener<T>(propertyName);
            listner.Observe(owner, action);
        }

        public void Observe<T>(string propertyName, Action<T, T> action)
        {
            Observe<T>(propertyName, this, action);
        }

        public void Observe<T>(Expression<Func<T>> property, object owner, Action<T, T> action)
        {
            Observe<T>(property.GetName(), owner, action);
        }

        public void Observe<T>(Expression<Func<T>> property, Action<T, T> action)
        {
            Observe(property, this, action);
        }

        #endregion

        #region All Versions of Unobserve

        public void Unobserve(string propertyName, object owner)
        {
            if (_hasListener(propertyName))
                _listeners[propertyName].Unobserve(owner);
        }

        public void Unobserve(string propertyName)
        {
            Unobserve(propertyName, this);
        }

        public void Unobserve<T>(Expression<Func<T>> property, object owner)
        {
            Unobserve(property.GetName(), owner);
        }

        public void Unobserve<T>(Expression<Func<T>> property)
        {
            Unobserve(property.GetName(), this);
        }

        public void Unobserve(string propertyName, object owner, Action action)
        {
            if (_hasListener(propertyName))
                _listeners[propertyName].Unobserve(owner, action);
        }

        public void Unobserve(string propertyName, Action action)
        {
            Unobserve(propertyName, this, action);
        }

        public void Unobserve<T>(Expression<Func<T>> property, object owner, Action action)
        {
            Unobserve(property.GetName(), owner, action);
        }

        public void Unobserve<T>(Expression<Func<T>> property, Action action)
        {
            Unobserve(property.GetName(), this, action);
        }

        public void Unobserve<T>(string propertyName, object owner, Action<T> action)
        {
            if (_hasListener(propertyName))
                _listeners[propertyName].Unobserve(owner, action);
        }

        public void Unobserve<T>(string propertyName, Action<T> action)
        {
            Unobserve(propertyName, this, action);
        }

        public void Unobserve<T>(Expression<Func<T>> property, object owner, Action<T> action)
        {
            Unobserve(property.GetName(), owner, action);
        }

        public void Unobserve<T>(Expression<Func<T>> property, Action<T> action)
        {
            Unobserve(property.GetName(), this, action);
        }

        public void Unobserve<T>(string propertyName, object owner, Action<T, T> action)
        {
            if (_hasListener(propertyName))
                _listeners[propertyName].Unobserve(owner, action);
        }

        public void Unobserve<T>(string propertyName, Action<T, T> action)
        {
            Unobserve(propertyName, this, action);
        }

        public void Unobserve<T>(Expression<Func<T>> property, object owner, Action<T, T> action)
        {
            Unobserve(property.GetName(), owner, action);
        }

        public void Unobserve<T>(Expression<Func<T>> property, Action<T, T> action)
        {
            Unobserve(property.GetName(), this, action);
        }

        #endregion
    }
}
