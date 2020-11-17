using ConsoleDemo.Samples.Disposables;
using log4net;
using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Builder;
using Unity.Extension;
using Unity.Injection;
using Unity.Policy;
using Unity.Resolution;
using Unity.Strategies;

namespace ConsoleDemo
{
    class Program
    {
        [STAThread]
        public static async Task Main(string[] args)
        {
            await ConsoleDemo.Samples.AsyncEventBug.Main.Run();
            Console.ReadLine();
        }
    }
}
