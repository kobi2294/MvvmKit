using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ComponentState
    {
        private List<(Action<object, object> setter, object value)> _setters;
        private Dictionary<string, object> _values;
        private Dictionary<MemberInfo, object> _memberValues;
        private Func<StateReader, Task> _onDestroyEntry = s => Task.CompletedTask;

        internal ComponentState()
        {
            _setters = new List<(Action<object, object> setter, object value)>();
            _values = new Dictionary<string, object>();
            _memberValues = new Dictionary<MemberInfo, object>();
        }

        internal ComponentState AddSetter(Action<object, object> setter, MemberInfo member, object value)
        {
            _setters.Add((setter, value));
            _memberValues.Add(member, value);
            return this;
        }

        internal IEnumerable<(Action<object, object> setter, object value)> GetSetters()
        {
            return _setters.ToList();
        }

        internal IEnumerable<(string key, object value)> GetValues()
        {
            return _values.Select(pair => (pair.Key, pair.Value));
        }

        internal IEnumerable<(MemberInfo key, object value)> GetMemberValues()
        {
            return _memberValues.Select(pair => (pair.Key, pair.Value));
        }

        internal ComponentState AddValue(string key, object value)
        {
            _values.Add(key, value);
            return this;
        }

        internal ComponentState SetDestroyEntry(Func<StateReader, Task> action)
        {
            _onDestroyEntry = action;
            return this;
        }

        internal Func<StateReader, Task> GetDestroyEntry()
        {
            return _onDestroyEntry;
        }

        internal object GetValue(string key)
        {
            return _values[key];
        }

        internal object GetMemberValue(MemberInfo member)
        {
            return _memberValues[member];
        }

        internal void Clear()
        {
            _setters.Clear();
            _values.Clear();
            _memberValues.Clear();
        }

    }
}
