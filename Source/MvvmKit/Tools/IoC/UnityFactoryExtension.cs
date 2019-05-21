using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Builder;
using Unity.Extension;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Strategies;

namespace MvvmKit
{
    [SecuritySafeCritical]
    public class UnityFactoryExtension<ResolvedType> : UnityContainerExtension
    {
        private Func<Type, ResolvedType> _fromDeclaringType;

        public UnityFactoryExtension()
        {
        }

        public UnityFactoryExtension<ResolvedType> WithFactory(Func<Type, ResolvedType> func)
        {
            _fromDeclaringType = func;
            return this;
        }

        protected override void Initialize()
        {
            Context.Policies.Set(typeof(ResolvedType), null, typeof(ResolveDelegateFactory), (ResolveDelegateFactory)GetResolver);
        }

        public ResolveDelegate<BuilderContext> GetResolver(ref BuilderContext context)
        {
            return (ref BuilderContext c) => _fromDeclaringType(c.DeclaringType);
        }
    }


}
