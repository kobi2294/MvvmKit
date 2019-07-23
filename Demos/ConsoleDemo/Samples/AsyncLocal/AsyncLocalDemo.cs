using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.AsyncLocal
{
    public static class AsyncLocalDemo
    {
        private static async Task _do()
        {
            // Ma nishma, yaniv?

        }

        public static Task Run()
        {
            AsyncLocal<int> alint = new AsyncLocal<int>();


            return Tasks.Empty;
        }
    }
}
