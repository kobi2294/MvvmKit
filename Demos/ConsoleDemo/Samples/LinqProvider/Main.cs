using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.LinqProvider
{
    public static class Main
    {
        public static void Run()
        {
            var myType = new MyType<int>();
            var yourType = new MyType<string>();
            var x = from y in yourType
                    from t in new MyType<int>()
                    let z = t
                    orderby z + 1 descending, t - 1, t ^ 2 descending
                    where t > 10
                    select t * 2;

            string str = x.ToString();
            Console.WriteLine(str);
        }
    }
}
