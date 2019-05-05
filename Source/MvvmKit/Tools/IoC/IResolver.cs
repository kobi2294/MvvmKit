using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public interface IResolver
    {
        T Resolve<T>();

        object Resolve(Type t);
    }
}
