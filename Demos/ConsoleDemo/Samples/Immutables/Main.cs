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

            state = state
                .With(x => x.Humans)
                .Find(p => p.Age == 44)
                .With(p => p.Friends)
                .Add(new Person(firstName: "Momo", "Levy", 22))
                .Replace(p => p.With(x => x.FirstName, x => x.FirstName + "..."));

            Print(state);

            var upsertables = ImmutableList.Create(
                new Person(firstName: "Adva", lastName: "Hari", age: 55),
                new Person(firstName: "Shimon", lastName: "Dahan", age: 37));

            state = state
                .With(x => x.Humans)
                .Upsert(upsertables, p => p.FirstName + p.LastName);

            Print(state);
        }

        public static void TestIf()
        {
            var state = new RootState(42, ImmutableList<Person>.Empty.Add(new NicePerson("Kobi", "Hari", 45, 10)));

            var state1 = state
                .With(x => x.Humans)
                .At(0)
                .If(person => person.Age > 40)
                .Set(x => x.Age, x => x.Age - 10)
                .Go();

            var state2 = state
                .With(x => x.Humans)
                .At(0)
                .If(person => person.Age < 40)
                .Set(x => x.Age, x => x.Age - 10)
                .Go();

            var state3 = state
                .With(x => x.Humans)
                .At(0)
                .Cast<NicePerson>()
                .If(np => np.HowNice == 10)
                .Set(x => x.HowNice, x => x.HowNice + 2)
                .Go();

            Print(state1);
            Print(state2);
            Print(state3);

        }

        public static void Print(RootState state)
        {
            var str = JsonConvert.SerializeObject(state, Formatting.Indented);
            Console.WriteLine(str);
            Console.WriteLine("--------------------------------------------------");
        }
    }
}
