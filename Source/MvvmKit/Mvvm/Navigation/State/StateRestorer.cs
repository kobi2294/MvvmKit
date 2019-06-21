using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class StateRestorer: BaseDisposable
    {
        private ComponentBase _owner;
        private ComponentState _state;

        public StateRestorer(ComponentBase owner, ComponentState state)
        {
            _owner = owner;
            _state = state;
        }

        public void RunSetters()
        {
            foreach (var pair in _state.GetSetters())
            {
                var setter = pair.setter;
                var value = pair.value;
                setter(_owner, value);
            }
        }

        public T Get<T>(string key)
        {
            Validate();
            return (T)_state.GetValue(key);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            _owner = null;
            _state = null;
        }
    }
}
