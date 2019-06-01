using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public interface ICollectionChangeClear : ICollectionChange
    {
    }

    public interface ICollectionChangeClear<T> : ICollectionChangeClear, ICollectionChange<T>
    {
    }
}
