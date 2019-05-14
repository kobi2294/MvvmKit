using ConsoleDemo.Samples.Disposables;
using log4net;
using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
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
        class Vm: BindableBase
        {
            private int _Num;
            public int Num { get { return _Num; } set { SetProperty(ref _Num, value); } }

        }


        static void Main(string[] args)
        {
            Samples.IoC.IoCTest.Run();
            // before new development

            var vm = new Vm();
            var x = new object();

            vm.Observe(() => vm.Num, (oldv, newv) => Console.WriteLine($"Value Changed from {oldv} to {newv}"));
            vm.Observe(() => vm.Num, x, (oldv, newv) => Console.WriteLine($"XXXXX!!! Value Changed from {oldv} to {newv}"));

            vm.Num = 20;
            vm.Num = 50;

            vm.Unobserve(() => vm.Num);
            vm.Num = 80;

            Console.ReadLine();
            
        }
    }
}
