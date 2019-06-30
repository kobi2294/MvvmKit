using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class StateWriter: BaseDisposable
    {
        private object _owner;
        private StateStore _state;
        private string _typeName;


        public StateWriter(object owner, StateStore state)
        {
            _owner = owner;
            _typeName = _owner.GetType().FullName;
            _state = state;
        }

        public void WriteMember<T>(Expression<Func<T>> member)
        {
            Validate();
            var me = member.GetMemberExpression();            
            var m = me.Member;
            var getter = m.ToGetter<object, object>();
            var setter = m.ToSetter<object, object>();

            var value = getter(_owner);
            _state.AddMember(m, setter, value);
        }

        public void WriteAnnotation<T>(string key, T value)
        {
            Validate();
            _state.AddAnnotation(key, value);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            _owner = null;
            _state = null;
        }

    }
}
