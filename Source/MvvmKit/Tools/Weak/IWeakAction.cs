using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public interface IWeakActionWithParam1
    {
        object Target
        {
            get;
        }

        void ExecuteWithObject(object parameter);

        void MarkForDeletion();
    }

    public interface IWeakActionWithParam2 : IWeakActionWithParam1
    {
        void ExecuteWithObject(object param1, object param2);
    }
}
