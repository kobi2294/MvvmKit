using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class StateReader: BaseDisposable
    {
        private StateStore _state;

        internal StateReader(StateStore state)
        {
            _state = state;
        }

        public T GetMember<T>(Expression<Func<T>> member)
        {
            Validate();
            var me = member.GetMemberExpression();
            var m = me.Member;
            var value = _state.Member<T>(m).value;
            return value;            
        }

        public T GetAnnotation<T>(string key)
        {
            Validate();
            var value = (T)_state.Annotation<T>(key);
            return value;
        }

        public IEnumerable<(string key, object value)> Dump()
        {
            var members = _state.Members().Select(x => (x.member.Name, x.value));
            var annotations = _state.Annotations();

            return members.Concat(annotations);
        }

        public void DumpToDebug(string title)
        {
            Debug.WriteLine(title);
            foreach (var item in Dump())
            {
                Debug.WriteLine($"{item.key}: {item.value}");
            }
        }
    }
}
