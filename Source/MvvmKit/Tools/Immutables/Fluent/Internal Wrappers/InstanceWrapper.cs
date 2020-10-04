using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Tools.Immutables.Fluent
{
    internal class InstanceWrapper<TRoot, T>: ImmutableInstanceWrapper<TRoot, T>, IInstanceModifier<T>
        where TRoot: class, IImmutable
        where T: class, IImmutable
    {
        private readonly List<IInstanceModifier<T>> _modifiers;
        private readonly RootWrapper<TRoot> _root;

        public Predicate<T> Predicate { get; }

        public InstanceWrapper(RootWrapper<TRoot> root, Predicate<T> predicate = null)
        {
            Predicate = predicate ?? (t => true);
            _modifiers = new List<IInstanceModifier<T>>();
            _root = root;
        }

        public T Modify(T source)
        {
            if (!Predicate(source)) return source;

            var current = source;
            foreach (var modifier in _modifiers)
            {
                current = modifier.Modify(current);
            }

            return current;
        }

        public override ImmutableListWrapper<TRoot, TObj> With<TObj>(Expression<Func<T, ImmutableList<TObj>>> expression)
        {
            var modifier = new InstanceWithListModifier<TRoot, T, TObj>(_root, expression);
            _modifiers.Add(modifier);
            return modifier.Target;
        }

        public override ImmutableInstanceWrapper<TRoot, TObj> With<TObj>(Expression<Func<T, TObj>> expression)
        {
            var modifier = new InstanceWithInstanceModifier<TRoot, T, TObj>(_root, expression);
            _modifiers.Add(modifier);
            return modifier.Target;
        }

        public override ImmutableInstanceWrapper<TRoot, T> Set<TVal>(Expression<Func<T, TVal>> expression, TVal value, Predicate<T> predicate = null)
        {
            var setter = new InstanceSetterModifier<T, TVal>(expression, value, predicate);
            _modifiers.Add(setter);
            return this;
        }

        public override ImmutableInstanceWrapper<TRoot, T> Set<TVal>(Expression<Func<T, TVal>> expression, Func<T, TVal> valueFunc, Predicate<T> predicate = null)
        {
            var setter = new InstanceSetterModifier<T, TVal>(expression, valueFunc, predicate);
            _modifiers.Add(setter);
            return this;
        }

        public override TRoot Go()
        {
            return _root.Go();
        }
    }
}
