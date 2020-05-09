using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Remute
{
    public class C : IImmutable, IWithGuid
    {
        public int Uid { get; }

        public string Caption { get; set; }

        public C(
            int uid= default, 
            string caption = ""
            )
        {
            Uid = uid;
            Caption = caption;
        }
    }
}
