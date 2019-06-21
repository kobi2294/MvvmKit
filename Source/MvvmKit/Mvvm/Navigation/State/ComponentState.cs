using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class ComponentState
    {
        private List<(Action<object, object> setter, object value)> _setters;
        private Dictionary<string, object> _values;


        public ComponentState()
        {
            _setters = new List<(Action<object, object> setter, object value)>();
            _values = new Dictionary<string, object>();
        }

        public void AddSetter(Action<object, object> setter, object value)
        {
            _setters.Add((setter, value));
        }

        public IEnumerable<(Action<object, object> setter, object value)> GetSetters()
        {
            return _setters.ToList();
        }

        public void AddValue(string key, object value)
        {
            _values.Add(key, value);
        }

        public object GetValue(string key)
        {
            return _values[key];
        }

        public void Clear()
        {
            _setters.Clear();
            _values.Clear();
        }

    }
}
