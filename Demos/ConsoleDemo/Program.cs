using ConsoleDemo.Samples.Disposables;
using log4net;
using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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

        static async Task Main(string[] args)
        {
            await Samples.ServiceState.Test.Run();

            Console.ReadLine();
        }
    }
}
