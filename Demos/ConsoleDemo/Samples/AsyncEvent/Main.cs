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

            await ae.Subscribe(_owner, OnAeChanged1);
            Console.WriteLine("Subscribe 1");

            await ae.Subscribe(_owner, OnAeChanged2);
            Console.WriteLine("Subscribe 2");

            await ae.Subscribe(_owner, OnAeChanged3);
            Console.WriteLine("Subscribe 3");

            await ae.Invoke(13);
           
            await ae.Unsubscribe(_owner, OnAeChanged1);
            Console.WriteLine("Unubscribe 1");

            await ae.Invoke(14);

            await ae.Unsubscribe(_owner);
            Console.WriteLine("Unubscribe owner");

            await ae.Invoke(15);
        }

        private static Task OnAeChanged1(int arg)
        {
            Console.WriteLine($"(1) Event thrown, with value: {arg}");
            return Task.CompletedTask;
        }

        private static Task OnAeChanged2(int arg)
        {
            Console.WriteLine($"(2) Event thrown, with value: {arg}");
            return Task.CompletedTask;
        }

        private static Task OnAeChanged3(int arg)
        {
            Console.WriteLine($"(3) Event thrown, with value: {arg}");
            return Task.CompletedTask;
        }
    }
}
