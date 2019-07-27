using MvvmKit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.ServiceState
{
    public static class Test
    {
        public static async Task Run()
        {
            var t = typeof(IStateCollection<int>);

            var store = new ServiceStore<IData>(data =>
            {
                data.Condition = false;
                data.Number = 42;
                data.Uid = "ABCC";
                data.Numbers.Reset(2, 5, 8, 12);
            });

            var res = await store.Select(data => $"{data.Uid}: {data.Condition}, {data.Number}\n {string.Join(", ", data.Numbers)}");
            Console.WriteLine(res);
            await store.Modify(data =>
            {
                data.Number = 50;
                data.Uid = data.Uid + data.Number;
                data.Numbers.AddRange(20, 40);
                data.Numbers.InsertRange(2, 50, 60);
            });
            res = await store.Select(data => $"{data.Uid}: {data.Condition}, {data.Number}\n {string.Join(", ", data.Numbers)}");
            Console.WriteLine(res);
        }
    }
}
