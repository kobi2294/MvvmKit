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
            var t = typeof(IStateList<int>);
            var service = new BackgroundServiceBase();

            var store = new ServiceStore<IData>(data =>
            {
                data.Condition = false;
                data.Number = 42;
                data.Uid = "ABCC";
                data.Numbers.Reset(2, 5, 8, 12);
            });

            var condProp = store.CreateWriter(service, x => x.Condition);
            var numbersProp = store.CreateWriter(service, x => x.Numbers);

            await condProp.Changed.Subscribe(store, b =>
            {
                Console.WriteLine("Condition changed to " + b);
                return Task.CompletedTask;
            });

            await condProp.Set(true);
            await condProp.Set(false);

            await store.Observe(d => d.Numbers).Subscribe(store, args =>
            {
                Console.WriteLine("Numbers Changes");
                Console.WriteLine(string.Join("\n", args));
                return Task.CompletedTask;
            });

            await store.Observe(d => d.Uid).Subscribe(store, args =>
            {
                Console.WriteLine("Uid changed to: " + args);
                return Task.CompletedTask;
            });

            await numbersProp.Modify(list =>
            {
                var i = list[2];
                list[2]++;
                list.MoveAt(1, 2);
                list.RemoveAt(3);
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
