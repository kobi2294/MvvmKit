using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ConsoleDemo.Samples.AsyncLocal
{
    public static class AsyncLocalDemo
    {
        static AsyncLocal<int> alint = new AsyncLocal<int>();

        private static async Task _do(string msg)
        {
            Console.WriteLine($"{msg}, data = {alint.Value} ");
            await Task.Delay(500);
            Console.WriteLine($"{msg}, data = {alint.Value} ");
        }

        public static async Task Run()
        {
            alint.Value = 42;
            await _do("First run");

            alint.Value = 50;

            var t1 = _do("Run 2.1");

            alint.Value = 60;

            var t2 = _do("Run 2.2");

            alint.Value = 70;

            var t3 = _do("Run 2.3");

            alint.Value = 80;

            var dis = Dispatcher.CurrentDispatcher;
            Console.WriteLine($"Main Dispatcher is: {dis}");
            Console.WriteLine($"Thread: {Thread.CurrentThread.ManagedThreadId}");

            await Task.Run(() =>
            {
                var d2 = Dispatcher.CurrentDispatcher;
                Console.WriteLine($"Background Dispatcher is: {d2}");
                Console.WriteLine($"Thread: {Thread.CurrentThread.ManagedThreadId}");

                Console.WriteLine($"Equality: {dis == d2}");
            });

            await Task.WhenAll(t1, t2, t3);
            
        }
    }
}
