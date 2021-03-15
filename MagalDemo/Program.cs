using MagalDemo.Entities;
using MvvmKit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagalDemo
{
    class Program
    {
        static void Main(string[] args)
        {

            var p = new Person(
                firstName: "John", 
                age:  42
                );

            p = new Person(firstName: p.FirstName,
                lastName: "Smith",
                age: p.Age,
                mainAddress: new Address(city: "City Chadash", country: p.MainAddress.Country),
                aliases: p.Aliases.Add("Jonathan"));

            // change last name
            p = p.With(x => x.LastName, "Cohen");
            p = p.With(x => x.Age, q => q.Age + 1);
            p = p.With(x => x.Aliases, q => q.Aliases.Add("Jonathan"));

            // creates 2 Person objects
            p = p.With(x => x.FirstName, "Paul")
                 .With(x => x.LastName, "McCartney");

            // creates 1 wrapper object and finally generates a Person with all the changes at once
            p = p.Set(x => x.FirstName, "Paul")
                .Set(x => x.LastName, "McCartney")
                .With(x => x.MainAddress)
                .Set(x => x.City, "Haifa")
                .Go();
                
        }

        private static IEnumerable<Person> _myPersonCreator()
        {
            for (int i = 0; i < 1000; i++)
            {
                yield return new Person();
            }
        }

        public static void FunWithImmutableCollections()
        {
            var l = ImmutableList.Create(1, 2, 3);
            var l1 = l.Add(5);

            var lofP = _myPersonCreator().ToImmutableList();

            var lofp2 = lofP.RemoveRange(300, 100);
        }

        public static void FunWithImmutableWrappers()
        {
            var t = new Team(
                name: "A Team");

            t = t.With(x => x.Members, ImmutableList.Create(
                new Person(firstName: "John", mainAddress: new Address(city: "Haifa"))
                ));

            t = t
                .With(x => x.Members)
                .At(0)
                .With(x => x.MainAddress)
                .Set(x => x.City, "Tel Aviv");

        }
    }
}
