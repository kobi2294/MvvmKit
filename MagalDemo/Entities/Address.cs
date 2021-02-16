using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagalDemo.Entities
{
    public class Address: IImmutable
    {
        public string City { get; }

        public string Country { get; }

        public Address(
            string city = "", 
            string country = ""
            )
        {
            City = city;
            Country = country;
        }
    }
}
