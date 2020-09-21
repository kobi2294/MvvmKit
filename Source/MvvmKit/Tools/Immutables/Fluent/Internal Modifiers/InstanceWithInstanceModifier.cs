using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Tools.Immutables.Fluent
{
    internal class InstanceWithInstanceModifier<TRoot, T, TTrg> : IInstanceModifier<T>
        where TRoot: class, IImmutable
        where T: class, IImmutable
        where TTrg: class, IImmutable
    {
        private readonly Expression<Func<T, TTrg>> _expression;
        internal InstanceWrapper<TRoot, TTrg> Target { get; }

        internal InstanceWithInstanceModifier(RootWrapper<TRoot> root,  Expression<Func<T, TTrg>> expression)
        {
            _expression = expression;
            Target = new InstanceWrapper<TRoot, TTrg>(root);
        }

        public T Modify(T source)
        {
            var getter = _expression.GetProperty().AsGetter<T, TTrg>();
            var target = getter(source);
            var modifiedTarget = Target.Modify(target);
            return source.With(_expression, modifiedTarget);
        }
    }
}
