using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo
{
    class Program
    {
        class Vm: Bindable
        {

            private int _Num;
            public int Num { get { return _Num; } set { SetProperty(ref _Num, value); } }

        }

        static void Main(string[] args)
        {
            // Master
            // before new development
            // Kobi

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
