using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Tools.Immutables.Fluent
{
    internal class ListReplaceModifier<T> : IListModifier<T>
        where T : class, IImmutable
    {
        private readonly Func<T, T> _projection;
        private readonly Func<T, int, T> _indexedProjection;
        private readonly bool _usesIndex = false;

        public ListReplaceModifier(Func<T, T> projection)
        {
            _projection = projection;
            _indexedProjection = null;
            _usesIndex = false;
        }

        public ListReplaceModifier(Func<T, int, T> projection)
        {
            _projection = null;
            _indexedProjection = projection;
            _usesIndex = true;
        }

        ImmutableList<T> IListModifier<T>.Modify(ImmutableList<T> source)
        {
            if (_usesIndex)
            {
                return source.Select(_indexedProjection).ToImmutableList();
            } else
            {
                return source.Select(_projection).ToImmutableList();
            }
        }
    }
}
