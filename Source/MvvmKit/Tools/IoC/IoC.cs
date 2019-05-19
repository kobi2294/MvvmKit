using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace MvvmKit
{
    public static class IoC
    {
        public static IUnityContainer AddFactoryFor<ResolvedType>(this IUnityContainer container, Func<Type, ResolvedType> factory)
        {
            container.AddExtension(new UnityFactoryExtension<ResolvedType>().WithFactory(factory));
            return container;
        }
    }
}
