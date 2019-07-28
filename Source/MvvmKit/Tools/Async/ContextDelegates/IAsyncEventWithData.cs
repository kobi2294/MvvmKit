using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    interface IAsyncEventWithData
    {
        Task Invoke(object value);
    }
}
