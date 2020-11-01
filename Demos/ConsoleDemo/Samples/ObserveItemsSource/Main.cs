using MvvmKit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ConsoleDemo.Samples.ObserveItemsSource
{
    public static class Main
    {
        public static void Run()
        {
            var oc = Enumerable.Range(1, 100).ToObservableCollection();
            var lb = new ListBox();

            lb.ObserveItemsChanged<int>()
                .Subscribe(val =>
                {
                    Console.WriteLine(val.Count);
                });

            var containersObservable = lb
                .ObserveItemsChanged<int>()
                .Select(items => items
                        .Select(item => lb.ItemContainerGenerator.ContainerFromItem(item))
                        .ToImmutableList())
                .Subscribe(containers =>
                {

                });


            lb.ItemsSource = Enumerable.Range(1, 100).ToObservableCollection();

            lb.ItemsSource = Enumerable.Range(1, 50).ToObservableCollection();
            lb.ItemsSource = oc;
            oc.Add(200);
            oc.Add(300);
            oc.Remove(200);
            oc.Remove(300);
            oc.Clear();
            oc.Add(1);
            lb.ItemsSource = null;

        }
    }
}
