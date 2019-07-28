using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.ServiceState
{
    public interface IData
    {
        string Uid { get; set; }

        int Number { get; set; }

        bool Condition { get; set; }

        IStateList<int> Numbers { get; set; }
    }
}
