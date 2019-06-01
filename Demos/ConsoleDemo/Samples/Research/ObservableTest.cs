using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Research
{
    public static class ObservableTest
    {
        public static void Run()
        {
            var oc = new ObservableCollection<int>() { 1, 10, 30, 100, 53, 99, 70 };
            oc.CollectionChanged += Oc_CollectionChanged;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Items: " + string.Join(", ", oc));
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Add(T)");
            oc.Add(10);
            oc.Add(20);

            Console.WriteLine("Insert(int, T)");
            oc.Insert(5, 200);
            oc.Insert(5, 300);

            Console.WriteLine("Remove");
            oc.Remove(30);
            oc.Remove(10);

            Console.WriteLine("Remove At");
            oc.RemoveAt(2);
            oc.RemoveAt(2);

            Console.WriteLine("Move");
            oc.Move(2, 4);
            oc.Move(3, 5);

            Console.WriteLine("Set index");
            oc[4] = 77;
            oc[4] = 88;

            Console.WriteLine("Clear");
            oc.Clear();

        }

        private static void Oc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var oldItems = "";
            if (e.OldItems != null) oldItems = string.Join(", ", e.OldItems.Cast<int>());

            var newItems = "";
            if (e.NewItems != null) newItems = string.Join(", ", e.NewItems.Cast<int>());

            var oc = sender as ObservableCollection<int>;
            var items = string.Join(", ", oc.Select(i => $"{i,5}"));
            var indexes = string.Join(", ", Enumerable.Range(0, oc.Count).Select(i => $"{i,5}"));


            Console.WriteLine($"Change: {e.Action}");
            Console.WriteLine($"    OldStartingIndex: {e.OldStartingIndex}");
            Console.WriteLine($"    Old Items: {oldItems}");

            Console.WriteLine($"    NewStartingIndex {e.NewStartingIndex}");
            Console.WriteLine($"    New Items: {newItems}");

            Console.WriteLine($"    Items Now: {items}");
            Console.WriteLine($"    Indices  : {indexes}");
            if (Console.ForegroundColor == ConsoleColor.White)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else
                Console.ForegroundColor = ConsoleColor.White;

        }
    }
}
