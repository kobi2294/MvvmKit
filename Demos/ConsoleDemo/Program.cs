﻿using ConsoleDemo.Samples.Disposables;
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
        public static void Main(string[] args)
        {
            ConsoleDemo.Samples.Disposables.Main.Run();
            Console.ReadLine();
        }
    }
}
