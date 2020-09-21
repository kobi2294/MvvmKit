using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class Signature
    {
        private class SignatureOf<DelegateType>
        {
            public static Type ReturnType { get; }

            public static ImmutableList<Type> ParameterTypes { get; }

            static SignatureOf()
            {
                var invokeMethod = typeof(DelegateType).GetMethod("Invoke");
                ReturnType = invokeMethod.ReturnType;
                ParameterTypes = invokeMethod
                    .GetParameters()
                    .Select(prm => prm.ParameterType)
                    .ToImmutableList();
            }
        }

        public static Signature Of<DelegateType>() => new Signature
        {
            OfDelegate = typeof(DelegateType),
            ReturnType = SignatureOf<DelegateType>.ReturnType,
            ParameterTypes = SignatureOf<DelegateType>.ParameterTypes
        };

        public Type ReturnType { get; private set; }

        public IEnumerable<Type> ParameterTypes { get; private set; }

        public Type OfDelegate { get; private set; }

        private Signature()
        {
        }
    }
}
