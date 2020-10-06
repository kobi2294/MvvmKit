using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Immutables
{
    public class NicePerson: Person
    {
        public int HowNice { get; }

        public NicePerson(
            string firstName = default, 
            string lastName = default, 
            int age = default, 
            int howNice = default)
            :base(firstName, lastName, age)
        {
            HowNice = howNice;
        }
    }
}
