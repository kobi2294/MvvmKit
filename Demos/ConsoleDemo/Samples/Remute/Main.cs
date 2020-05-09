using MvvmKit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Remute
{
    public static class Main
    {
        public static void Run()
        {
            var prop = typeof(A).GetProperty("Number");
            var a = new A(4,
                new B(1, "Hello", 42, new C(1, "Hello C")),
                ImmutableList.Create(new B(1, "World", 52), new B(1, "Wusssup", 60)));

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(a.ToJson());

            a = a.ModifyRecusively((B b) => 
                    b.MyC != null
                    ? b
                    : b.With(x => x.MyC, new C(2, "Auto")));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(a.ToJson());


            a = a.ModifyRecusively((C c) => c.With(x => x.Uid, 3));

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(a.ToJson());
        }
    }
}
