using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Weaks
{
    public class Listener
    {
        public void ListenTo(Performer performer)
        {
            performer.OnPerformCallback(this, () =>
            {
                Console.WriteLine("Listener reacting to performer object");
            });
        }
    }


    public class Performer
    {
        WeakAction weakCallback;

        public void OnPerformCallback(object owner, Action callback)
        {
            weakCallback = callback.ToWeak(owner);
        }

        public void Perform()
        {
            Console.WriteLine($"IsAlive={weakCallback.IsAlive}");
            weakCallback.Execute();
        }
    }

    public static class WeakDelegateDemo
    {
        public static void Run()
        {
            var listener = new Listener();
            var performer = new Performer();
            listener.ListenTo(performer);

            Console.WriteLine("Calling delegate");
            performer.Perform();

            GC.Collect();
            Console.WriteLine("Calling delegate after garbage collection while still referencing listener");
            performer.Perform();

            listener = null;
            GC.Collect();
            Console.WriteLine("Calling delegate after nulling listener and garbage collection");
            performer.Perform();

            Console.WriteLine("Demo Completed");
        }
    }

}
