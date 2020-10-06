using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Tools.Immutables.Fluent
{
    internal class InstanceReplacerModifier<T>: IInstanceModifier<T>
        where T: class, IImmutable
    {
        private readonly Func<T, T> _valueFunc;

        private readonly T _value;

        private readonly bool _isUsingFunc;

        public Predicate<T> Predicate { get; }

        public InstanceReplacerModifier(T value, Predicate<T> predicate = null)
        {
            _value = value;
            _isUsingFunc = false;
            Predicate = predicate ?? (t => true);
        }

        public InstanceReplacerModifier(Func<T, T> valueFunc, Predicate<T> predicate = null)
        {
            _valueFunc = valueFunc;
            _isUsingFunc = true;
            Predicate = predicate ?? (t => true);
        }

        public T Modify(T source)
        {
            if (!Predicate(source)) return source;

            if (_isUsingFunc)
                return _valueFunc(source);
            else
                return _value;
        }
    }
}
