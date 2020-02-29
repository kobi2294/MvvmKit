using MvvmKit;
using System;
using System.Collections.Generic;
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

        public Person(
            string firstName, 
            string lastName, 
            int age)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
        }
    }
}
