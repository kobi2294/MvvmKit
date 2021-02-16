using MvvmKit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagalDemo.Entities
{
    public class Person: IImmutable
    {
        public string FirstName { get; }

        public string LastName { get;}

        public int Age { get; }

        public Address MainAddress { get; }

        public ImmutableList<string> Aliases { get; }

        public Person(
            string firstName = "",
            string lastName = "", 
            int age = 0, 
            Address mainAddress = default, 
            ImmutableList<string> aliases = default
            )
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            MainAddress = mainAddress ?? new Address();
            Aliases = aliases ?? ImmutableList<string>.Empty;
        }
    }
}
