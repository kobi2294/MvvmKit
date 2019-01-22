using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public interface IWeakFunc
    {
        object Target
        {
            get;
        }

        object ExecuteWithObject(object parameter);

        void MarkForDeletion();
    }

    public interface IWeakFunc<TResult>
    {
        TResult ExecuteWithObject(object parameter);
    }
}
