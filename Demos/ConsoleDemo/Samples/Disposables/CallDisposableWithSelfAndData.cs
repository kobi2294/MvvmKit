using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Disposables
{
    public static class CallDisposableWithSelfAndData
    {
        public static void Run()
        {
            var disposable = MvvmKit.Disposables.Call(OnDispose, 42);

            using (disposable)
            {
                Console.WriteLine("Using disposble with data: " + disposable.Data);
            }
        }

        public static void OnDispose(IDisposableWithData<int> disposable)
        {
            Console.WriteLine("Disposing object with data: " + disposable.Data);
        }
    }
}
