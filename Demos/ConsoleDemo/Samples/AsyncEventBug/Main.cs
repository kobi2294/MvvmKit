using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.AsyncEventBug
{
    public static class Main
    {
        private static object _mutex = new object();

        public class Service
        {
            public AsyncEvent<int> Changed = new AsyncEvent<int>();

            public Service()
            {
                Task.Factory.StartNew(async () =>
                {
                    var rnd = new Random();
                    var i = 0;
                    while(true)
                    {
                        i++;
                        await Task.Delay(rnd.Next(100, 1000));
                        var t = Changed.Invoke(i);
                    }
                });
            }
        }

        public class Consumer
        {
            private string _id;
            private ConsoleColor _color;

            public Consumer(string id, ConsoleColor color)
            {
                _id = id;
                _color = color;
            }

            public async Task Start(Service service)
            {
                await Task.Factory.StartNew(async () =>
                {
                    Print("Subscribe Start", _color, _id);
                    await service.Changed.Subscribe(this, async val =>
                    {
                        Print(val.ToString(), _color, _id);
                        await Task.Delay(1000);
                    });
                    Print("Subscribe End", _color, _id);
                });
            }
        }


        public static async Task Run()
        {
            var service = new Service();
            var consumer1 = new Consumer("First", ConsoleColor.Green);
            var consumer2 = new Consumer("Second", ConsoleColor.Red);

            //await Task.WhenAll(
            //    consumer1.Start(service),
            //    consumer2.Start(service)
            //    );

            //await consumer1.Start(service);
            //await consumer2.Start(service);

            var t1 = consumer1.Start(service);
            var t2 = consumer2.Start(service);

            await Task.Delay(10);



        }

        public static void Print(string text, ConsoleColor color, string prefix)
        {
            lock (_mutex)
            {
                Console.ForegroundColor = color;
                Console.WriteLine($"{prefix} {text}");
            }
        }
    }
}
