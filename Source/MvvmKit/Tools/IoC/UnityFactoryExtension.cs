using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Unity.Builder;
using Unity.Extension;
using Unity.Policy;
using Unity.Resolution;

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
            Type declaringType = context.DeclaringType;

            return (ref BuilderContext c) => _fromDeclaringType(declaringType);
        }
    }


}
