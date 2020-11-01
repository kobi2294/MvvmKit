using MvvmKit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Design;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SelectiveResourcesDemo.ItemsControlObservable
{
    /// <summary>
    /// Interaction logic for ItemsControlDemo.xaml
    /// </summary>
    public partial class ItemsControlDemo : UserControl
    {
        private ConsoleWindow _console;

        public ItemsControlDemo()
        {
            InitializeComponent();
            ConsoleWindow.IsConsoleWindowEnabled = true;
            _console = ConsoleWindow.CreateAndShow("Items Control Demo", Colors.DarkKhaki);

            lb.ObserveItemsChanged<int>()
                .Subscribe(val =>
                {
                    _console.WriteLine(val.Count.ToString(), "next");
                });

            lb.ObserveItemsChanged<int>()
                .Select(items => items
                        .Select(item => lb.ItemContainerGenerator.ContainerFromItem(item))
                        .ToImmutableList())
                .Subscribe(containers =>
                {
                    var txt = containers
                        .Select(c => c?.ToString() ?? "null")
                        .Join(", ");
                    _console.WriteLine(txt, "containers count");
                });

            lb.ObserveItemsGenerator<ListBoxItem>()
                .Subscribe(ctrls =>
                {
                    var txt = ctrls
                        .Select(depo => depo?.DataContext?.ToString() ?? "null")
                        .Join(", ");

                    _console.WriteLine(ctrls.Count().ToString(), "containers list count");
                    _console.WriteLine(ctrls.Where(x => x != null).Count().ToString(), "non null list containers");
                    _console.WriteLine(txt, "containers list");
                });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var oc = Enumerable.Range(1, 100).ToObservableCollection();

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
            lb.ItemsSource = Enumerable.Range(1, 100).ToObservableCollection();

        }
    }
}
