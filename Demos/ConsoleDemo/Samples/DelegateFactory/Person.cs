using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.DelegateFactory
{
    public class Person
    {
        public static Person CreateFromName(string firstName, string lastName)
        {
            return new Person { Name = $"{firstName} {lastName}" };
        }

        public string Name { get; set; }

        public Person()
        {
        }

        public Person(string firstName, string lastName)
        {
            Name = $"{lastName}, {firstName}";
        }

        public int CalcAge(string something)
        {
            return Name.Length + something.Length;
        }

        public override string ToString()
        {
            return $"Person: {Name}";
        }
    }
}
