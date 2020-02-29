using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Tools.Immutables.Fluent
{
    internal class RootWrapper<TRoot>: ImmutableWrapper<TRoot>
        where TRoot: class, IImmutable
    {
        internal TRoot _instance { get; }
        internal InstanceWrapper<TRoot, TRoot> Target { get; }

        public RootWrapper(TRoot instance)
        {
            _instance = instance;
            Target = new InstanceWrapper<TRoot, TRoot>(this);
        }

        public override TRoot Go()
        {
            return Target.Modify(_instance);
        }
    }
}
