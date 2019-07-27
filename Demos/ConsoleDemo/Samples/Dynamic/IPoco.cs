using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Dynamic
{
    public interface IPoco
    {
        int Number { get; set; }

        string Text { get; set; }

        bool Condition { get; set; }
    }
}
