using MvvmKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Immutables
{
    public static class Main
    {
        public static void Run()
        {
            var state = new RootState(42, ImmutableList<Person>.Empty.Add(new Person("Kobi", "Hari", 45)));

            Print(state);

            state = state
                .Set(x => x.A, 50)
                .With(x => x.Humans)
                .Add(
                    new Person("Ronni", "Hari", 2),
                    new Person("Adva", "Hari", 43),
                    new Person("Elad", "Perltzweig", 13),
                    new Person("John", "Smith", 60));

            Print(state);

            state = state
                .With(x => x.Humans)
                .Remove(p => p.FirstName == "John")
                .Find(p => p.Age == 45)
                .Set(x => x.FirstName, "Yakov")
                .Set(x => x.Age, 44);

            Print(state);
        }

        public static void Print(RootState state)
        {
            var str = JsonConvert.SerializeObject(state, Formatting.Indented);
            Console.WriteLine(str);
            Console.WriteLine("--------------------------------------------------");
        }
    }
}
