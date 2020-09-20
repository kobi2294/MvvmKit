using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.DelegateFactory
{
    public class ImmutableShape: IImmutable
    {
        public int A { get; }

        public string B { get; }

        public double C { get; }

        public Person D { get; }

        public ImmutableShape(
            int a = 0, 
            string b = "", 
            double c = 0.0, 
            Person d = null
            )
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }
    }
}
