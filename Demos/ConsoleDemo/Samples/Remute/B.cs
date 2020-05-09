using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Remute
{
    public class B: IImmutable, IWithGuid
    {
        public int Uid { get; }

        public string MyValue { get;  }

        public int MyInt { get;  }

        public C MyC { get; set; }


        public B(
            int uid = default,
            string myValue = "", 
            int myInt = 0, 
            C myC = null
            )
        {
            Uid = uid;
            MyValue = myValue;
            MyInt = myInt;
            MyC = myC;
        }
    }
}
