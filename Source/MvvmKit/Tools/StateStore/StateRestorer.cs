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
        private object _target;
        private StateStore _state;

        public StateRestorer(object target, StateStore state)
        {
            _target = target;
            _state = state;
        }

        public void RunSetters()
        {
            Validate();
            foreach (var record in _state.Members())
            {
                var setter = record.setter;
                var value = record.value;
                setter.Invoke(_target, value);
            }
        }

        public T Annotation<T>(string key)
        {
            Validate();
            return _state.Annotation<T>(key);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            _target = null;
            _state = null;
        }
    }
}
