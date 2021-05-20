using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Disposables
{
    public static class Main
    {
        public static void Run()
        {
            using (var dsource = new NotifyDisposableImpl())
            {

                var dl = new List<IDisposable>
                {
                    MvvmKit.Disposables.Call(() =>
                    {
                        Console.WriteLine("Item 1 in list disposed");
                    }),
                    MvvmKit.Disposables.Call(() =>
                    {
                        Console.WriteLine("Item 2 in list disposed");
                    }),
                    MvvmKit.Disposables.Call(() =>
                    {
                        Console.WriteLine("Item 3 in list disposed");
                    })
                }.AllDisposedBy(dsource);

                var ob = dsource.AsObservable(() => "42!!!");

                ob.Subscribe(val => Console.WriteLine("Observable value: " + val), () => Console.WriteLine("Observable Completed"));


                var dobj = MvvmKit.Disposables.Call(() =>
                {
                    Console.WriteLine("Disposing dobj");
                })
                .WhenDisposed(() => Console.WriteLine("and responding to it"))
                .DisposedBy(dsource);

                
            }

        }
    }
}
