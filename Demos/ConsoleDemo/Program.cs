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
            var vm = new Vm();

            var pch = vm.Properties[nameof(vm.Num)];
            pch.Observe<int>(vm, (o, n) => Console.WriteLine("Value changed from " + o + "to " + n));

            vm.Num = 20;
            vm.Num = 50;
            Console.ReadLine();
            
        }
    }
}
