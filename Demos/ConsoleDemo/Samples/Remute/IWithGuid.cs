using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Remute
{
    interface IWithGuid: IImmutable
    {
        int Uid { get; }
    }
}
