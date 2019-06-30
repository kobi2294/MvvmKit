using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class StateStore: BaseDisposable
    {
        private Dictionary<MemberInfo, (Action<object, object> setter, object value)> _memberValues;
        private Dictionary<string, object> _annotations;

        private StateStore()
        {
            _annotations = new Dictionary<string, object>();
            _memberValues = new Dictionary<MemberInfo, (Action<object, object> setter, object value)>();
        }

        internal StateStore AddMember(MemberInfo member, Action<object, object> setter, object value)
        {
            Validate();
            _memberValues.Add(member, (setter, value));
            return this;
        }

        internal StateStore AddAnnotation(string key, object value)
        {
            Validate();
            _annotations.Add(key, value);
            return this;
        }

        internal IEnumerable<(MemberInfo member, Action<object, object> setter, object value)> Members()
        {
            Validate();
            return _memberValues
                .Select(pair => (pair.Key, pair.Value.setter, pair.Value.value))
                .ToArray();
        }

        internal IEnumerable<(string key, object value)> Annotations()
        {
            Validate();
            return _annotations.Select(pair => (pair.Key, pair.Value))
                .ToArray();
        }

        internal object Annotation(string key)
        {
            Validate();
            return _annotations[key];
        }

        internal T Annotation<T>(string key)
        {
            Validate();
            return (T)_annotations[key];
        }

        internal (Action<object, object> setter, object value) Member(MemberInfo member)
        {
            Validate();
            return _memberValues[member];
        }

        internal (Action<object, T> setter, T value) Member<T>(MemberInfo member)
        {
            Validate();
            var pair = _memberValues[member];
            var value = (T)pair.value;
            Action<object, T> setter = (object target, T val) => pair.setter(target, val);
            return (setter, value);
        }

        public void Clear()
        {
            Validate();
            _annotations.Clear();
            _memberValues.Clear();
        }

        protected override void OnDisposed()
        {
            Clear();
            base.OnDisposed();
        }

        public static StateStore Write(object source, Action<StateWriter> write)
        {
            var state = new StateStore();
            using (var writer = new StateWriter(source, state))
            {
                write(writer);
            }
            return state;
        }

        public static async Task<StateStore> Write(object source, Func<StateWriter, Task> write)
        {
            var state = new StateStore();
            using (var writer = new StateWriter(source, state))
            {
                await write(writer);
            }
            return state;
        }

        public void Restore(object target, Action<StateRestorer> restore)
        {
            using (var restorer = new StateRestorer(target, this))
            {
                restorer.RunSetters();
                restore(restorer);
            }
        }

        public async Task Restore(object target, Func<StateRestorer, Task> restore)
        {
            using (var restorer = new StateRestorer(target, this))
            {
                restorer.RunSetters();
                await restore(restorer);
            }
        }

        public void Read(Action<StateReader> read)
        {
            using (var reader = new StateReader(this))
            {
                read(reader);
            }
        }

        public async Task Read(Func<StateReader, Task> read)
        {
            using (var reader = new StateReader(this))
            {
                await read(reader);
            }
        }
    }
}
