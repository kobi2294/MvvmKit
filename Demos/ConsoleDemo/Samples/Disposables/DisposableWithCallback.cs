using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Disposables
{
    public static class DisposableWithCallback
    {
        public static void Run()
        {
            // create a disposable that will write "Disposed!!!" to the console after it was disposed
            var disposable = MvvmKit.Disposables.Call(() => Console.WriteLine("Disposed!!!"));
            using (disposable)
            {
                Console.WriteLine("Using Disposble");
            }
        }
    }
}
