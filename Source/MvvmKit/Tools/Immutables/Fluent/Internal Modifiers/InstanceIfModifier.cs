using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Tools.Immutables.Fluent
{
    public class InstanceIfModifier<TRoot, T> : IInstanceModifier<T>
        where TRoot: class, IImmutable
        where T : class, IImmutable
    {
        public Predicate<T> Predicate { get; }

        internal InstanceWrapper<TRoot, T> Target { get; }

        internal InstanceIfModifier(RootWrapper<TRoot> root, Predicate<T> predicate)
        {
            Predicate = predicate ?? (t => true);
            Target = new InstanceWrapper<TRoot, T>(root);
        }


        public T Modify(T source)
        {
            if (!Predicate(source)) return source;
            return Target.Modify(source);
        }
    }
}
