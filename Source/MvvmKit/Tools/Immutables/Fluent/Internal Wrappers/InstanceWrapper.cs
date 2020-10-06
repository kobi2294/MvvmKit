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

        public InstanceWrapper(RootWrapper<TRoot> root)
        {
            _modifiers = new List<IInstanceModifier<T>>();
            _root = root;
        }

        public T Modify(T source)
        {
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

        public override ImmutableInstanceWrapper<TRoot, T> Set<TVal>(Expression<Func<T, TVal>> expression, TVal value)
        {
            var setter = new InstanceSetterModifier<T, TVal>(expression, value);
            _modifiers.Add(setter);
            return this;
        }

        public override ImmutableInstanceWrapper<TRoot, T> Set<TVal>(Expression<Func<T, TVal>> expression, Func<T, TVal> valueFunc)
        {
            var setter = new InstanceSetterModifier<T, TVal>(expression, valueFunc);
            _modifiers.Add(setter);
            return this;
        }
        public override ImmutableInstanceWrapper<TRoot, T> Replace(T value)
        {
            var modifier = new InstanceReplacerModifier<T>(value);
            _modifiers.Add(modifier);
            return this;
        }

        public override ImmutableInstanceWrapper<TRoot, T> Replace(Func<T, T> value)
        {
            var modifier = new InstanceReplacerModifier<T>(value);
            _modifiers.Add(modifier);
            return this;
        }

        public override ImmutableInstanceWrapper<TRoot, T> If(Predicate<T> predicate)
        {
            var modifier = new InstanceIfModifier<TRoot, T>(_root, predicate);
            _modifiers.Add(modifier);
            return modifier.Target;
        }

        public override ImmutableInstanceWrapper<TRoot, TCast> Cast<TCast>()
        {
            var modifier = new InstanceCastModifier<TRoot, T, TCast>(_root);
            _modifiers.Add(modifier);
            return modifier.Target;
        }

        public override TRoot Go()
        {
            return _root.Go();
        }

    }
}
