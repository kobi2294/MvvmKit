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
        private ComponentState _state;

        internal StateReader(ComponentState state)
        {
            _state = state;
        }

        public T Get<T>(Expression<Func<T>> member)
        {
            Validate();
            var me = member.GetMemberExpression();
            var m = me.Member;
            var value = (T)_state.GetMemberValue(m);
            return value;            
        }

        public T Get<T>(string key)
        {
            Validate();
            var value = (T)_state.GetValue(key);
            return value;
        }

        public IEnumerable<(string key, object value)> Dump()
        {
            var members = _state.GetMemberValues().Select(x => (x.key.Name, x.value));
            var values = _state.GetValues();

            return members.Concat(values);
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
