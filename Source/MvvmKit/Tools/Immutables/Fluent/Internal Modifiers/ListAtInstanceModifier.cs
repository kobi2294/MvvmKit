using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Tools.Immutables.Fluent
{
    internal class ListAtInstanceModifier<TRoot, T> : IListModifier<T>
        where TRoot : class, IImmutable
        where T : class, IImmutable
    {
        private int _index;
        internal InstanceWrapper<TRoot, T> Target { get; }

        public ListAtInstanceModifier(RootWrapper<TRoot> root, int index)
        {
            _index = index;
            Target = new InstanceWrapper<TRoot, T>(root);
        }

        public ImmutableList<T> Modify(ImmutableList<T> source)
        {
            var item = source[_index];
            var modifiedItem = Target.Modify(item);

            return source.SetItem(_index, modifiedItem);
        }
    }
}
