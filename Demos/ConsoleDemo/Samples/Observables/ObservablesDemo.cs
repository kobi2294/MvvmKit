using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Observables
{
    public static class ObservablesDemo
    {
        public static void Run()
        {
            var subj = new Subject<char>();
            var isRunning = true;

            var swt = subj
                .Select(c => Observable.Timer(TimeSpan.FromSeconds(2)))
                .Switch()
                .Select(x => false);

            var curVal = subj.Select(x => true).Merge(swt);

            curVal.Subscribe(x => Console.WriteLine("Switch emitted: " + x));


            subj.Subscribe(val => Console.WriteLine("Subject emitted: " + val));

            var t = Task.Factory.StartNew(() =>
            {
                while(isRunning)
                {
                    var c = Console.ReadKey();
                    subj.OnNext(c.KeyChar);
                    if (c.KeyChar == 'z') subj.OnCompleted();
                }
            });



            t.Wait();
        }

    }
}
