using MvvmKit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagalDemo.Entities
{
    public class Team: IImmutable
    {
        public string Name { get; }

        public ImmutableList<Person> Members { get; }

        public Team(
            string name = "",
            ImmutableList<Person> members = default
            )
        {
            Name = name;
            Members = members ?? ImmutableList<Person>.Empty;
        }
    }
}
