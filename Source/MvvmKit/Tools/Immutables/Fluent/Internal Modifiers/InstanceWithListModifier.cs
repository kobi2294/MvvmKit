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

        public Predicate<T> Predicate { get; }

        public InstanceWithListModifier(RootWrapper<TRoot> root, Expression<Func<T, ImmutableList<TTrg>>> expression, Predicate<T> predicate = null)
        {
            _expression = expression;
            Target = new ListWrapper<TRoot, TTrg>(root);
            Predicate = predicate ?? (t => true);

        }

        public T Modify(T source)
        {
            if (!Predicate(source)) return source;

            var getter = _expression.GetProperty().AsGetter<T, ImmutableList<TTrg>>();
            var target = getter(source);
            var modifiedTarget = Target.Modify(target);
            return source.With(_expression, modifiedTarget);

        }
    }
}
