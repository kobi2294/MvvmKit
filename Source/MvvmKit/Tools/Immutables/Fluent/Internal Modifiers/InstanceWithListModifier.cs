using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Tools.Immutables.Fluent
{
    internal class InstanceWithListModifier<TRoot, T, TTrg> : IInstanceModifier<T>
        where TRoot: class, IImmutable
        where T: class, IImmutable
        where TTrg: class, IImmutable
    {
        private readonly Expression<Func<T, ImmutableList<TTrg>>> _expression;
        internal ListWrapper<TRoot, TTrg> Target { get; }

        public InstanceWithListModifier(RootWrapper<TRoot> root, Expression<Func<T, ImmutableList<TTrg>>> expression)
        {
            _expression = expression;
            Target = new ListWrapper<TRoot, TTrg>(root);
        }

        public T Modify(T source)
        {
            var getter = _expression.GetProperty().ToGetter<T, ImmutableList<TTrg>>();
            var target = getter(source);
            var modifiedTarget = Target.Modify(target);
            return source.With(_expression, modifiedTarget);

        }
    }
}
