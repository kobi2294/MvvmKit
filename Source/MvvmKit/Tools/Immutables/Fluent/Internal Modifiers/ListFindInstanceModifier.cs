using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Tools.Immutables.Fluent
{
    internal class ListFindInstanceModifier<TRoot, T> : IListModifier<T>
        where TRoot: class, IImmutable
        where T: class, IImmutable
    {
        private readonly Predicate<T> _predicate;
        internal InstanceWrapper<TRoot, T> Target { get; }

        public ListFindInstanceModifier(RootWrapper<TRoot> root,  Predicate<T> predicate)
        {
            _predicate = predicate;
            Target = new InstanceWrapper<TRoot, T>(root);
        }

        public ImmutableList<T> Modify(ImmutableList<T> source)
        {
            var index = source.IndexOf(t => _predicate(t));
            if (index < 0) return source;
            var item = source[index];
            var modifiedItem = Target.Modify(item);

            return source.SetItem(index, modifiedItem);
        }
    }
}
