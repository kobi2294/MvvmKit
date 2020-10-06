using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Tools.Immutables.Fluent
{
    public class InstanceCastModifier<TRoot, T, TCast> : IInstanceModifier<T>
        where TRoot: class, IImmutable
        where T: class, IImmutable
        where TCast: class, IImmutable
    {
        internal InstanceWrapper<TRoot, TCast> Target { get; }

        internal InstanceCastModifier(RootWrapper<TRoot> root)
        {
            Target = new InstanceWrapper<TRoot, TCast>(root);
        }

        public T Modify(T source)
        {
            var casted = source as TCast;
            if (casted == null) return source;

            return Target.Modify(casted) as T;
        }
    }
}
