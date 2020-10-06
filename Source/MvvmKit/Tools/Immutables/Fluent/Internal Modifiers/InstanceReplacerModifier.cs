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

        public InstanceReplacerModifier(T value)
        {
            _value = value;
            _isUsingFunc = false;
        }

        public InstanceReplacerModifier(Func<T, T> valueFunc)
        {
            _valueFunc = valueFunc;
            _isUsingFunc = true;
        }

        public T Modify(T source)
        {
            if (_isUsingFunc)
                return _valueFunc(source);
            else
                return _value;
        }
    }
}
