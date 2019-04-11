using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Mvvm.ChangeListeners
{
    public class PropertyChangeListenerSource<T>
    {
        public PropertyChangeListener<T> Listener { get; }

        public PropertyChangeListenerSource()
        {
            Listener = new PropertyChangeListener<T>();
        }

        public void Invoke(T oldValue, T newValue)
        {
            Listener.Invoke(oldValue, newValue);
        }
    }
}
