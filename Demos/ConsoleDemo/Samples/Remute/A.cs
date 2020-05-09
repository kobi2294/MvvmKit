using MvvmKit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Remute
{
    public class A: IImmutable
    {
        public int Number { get; }

        public B SingleB { get; }

        public ImmutableList<B> ManyB { get; set; }

        public A(
            int number = 0, 
            B singleB = null, 
            ImmutableList<B> manyB = null)
        {
            Number = number;
            SingleB = singleB;
            ManyB = manyB ?? ImmutableList<B>.Empty;
        }
    }
}
