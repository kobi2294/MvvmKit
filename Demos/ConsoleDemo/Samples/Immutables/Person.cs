using MvvmKit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Immutables
{
    public class Person: IImmutable
    {
        public string FirstName { get; }

        public string LastName { get;  }

        public int Age { get;  }

        public ImmutableList<Person> Friends { get; }

        public Person(
            string firstName = default, 
            string lastName = default, 
            int age = default, 
            ImmutableList<Person> friends = null
            )
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            Friends = friends ?? ImmutableList<Person>.Empty;
        }
    }
}
