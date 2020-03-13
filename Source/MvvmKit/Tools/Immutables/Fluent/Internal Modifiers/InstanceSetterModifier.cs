using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Tools.Immutables.Fluent
{
    internal class InstanceSetterModifier<T, TVal> : IInstanceModifier<T>
        where T: class, IImmutable
    {
        private readonly Expression<Func<T, TVal>> _expression;

        private readonly TVal _value;

        private readonly Func<T, TVal> _valueFunc;

        private readonly bool _isUsingFunc;


        public InstanceSetterModifier(Expression<Func<T, TVal>> expression, TVal value)
        {
            _expression = expression;
            _value = value;
            _isUsingFunc = false;
        }

        public InstanceSetterModifier(Expression<Func<T, TVal>> expression, Func<T, TVal> valueFunc)
        {
            _expression = expression;
            _valueFunc = valueFunc;
            _isUsingFunc = true;
        }

        public T Modify(T source)
        {
            if (_isUsingFunc)
                return source.With(_expression, _valueFunc);
            else
                return source.With(_expression, _value);
        }
    }
}
