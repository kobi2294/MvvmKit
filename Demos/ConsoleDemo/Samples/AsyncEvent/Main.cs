using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.AsyncEvent
{
    public static class Main
    {
        private static object _owner = new object();

        public static async Task Run()
        {
            var ae = new AsyncEvent<int>(12);

            await ae.Subscribe(_owner, OnAeChanged);

            Console.WriteLine("Subscribe");
            await ae.Invoke(13);
           
            await ae.Unsubscribe(_owner, OnAeChanged);
            Console.WriteLine("Unubscribe specific");

            await ae.Invoke(14);

            await ae.Unsubscribe(_owner);
            Console.WriteLine("Unubscribe owner");

            await ae.Invoke(15);
        }

        private static Task OnAeChanged(int arg)
        {
            Console.WriteLine($"Event thrown, with value: {arg}");
            return Task.CompletedTask;
        }
    }
}
