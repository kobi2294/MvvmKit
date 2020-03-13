using MvvmKit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Immutables
{
    public class RootState: IImmutable
    {
        public int A { get; }

        public ImmutableList<Person> Humans { get; }

        public RootState(
            int a, 
            ImmutableList<Person> humans = null)
        {
            A = a;
            Humans = humans ?? ImmutableList<Person>.Empty;
        }
    }
}
