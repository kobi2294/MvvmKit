using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class StateSaver: BaseDisposable
    {
        private ComponentBase _owner;
        private ComponentState _state;
        private string _typeName;


        public StateSaver(ComponentBase owner)
        {
            _owner = owner;
            _typeName = _owner.GetType().FullName;
            _state = new ComponentState();
        }

        public void Save<T>(Expression<Func<T>> member)
        {
            Validate();
            var me = member.GetMemberExpression();            
            var m = me.Member;
            var getter = m.ToGetter<object, object>();
            var setter = m.ToSetter<object, object>();

            var value = getter(_owner);
            _state.AddSetter(setter, value);
        }

        public void Set<T>(string key, T value)
        {
            Validate();
            _state.AddValue(key, value);
        }

        internal ComponentState GetState()
        {
            return _state;
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            _owner = null;
            _state = null;
        }

    }
}
