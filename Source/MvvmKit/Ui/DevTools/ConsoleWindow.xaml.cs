using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MvvmKit
{
    /// <summary>
    /// Interaction logic for ConsoleWindow.xaml
    /// </summary>
    public partial class ConsoleWindow : Window
    {
        public ConsoleWindow()
        {
            InitializeComponent();
            txt.Text = "";
        }

        public static ConsoleWindow CreateAndShow()
        {
            var tsc = new TaskCompletionSource<ConsoleWindow>();

            Thread thread = new Thread(() =>
            {
                var win = new ConsoleWindow();
                win.Show();

                win.Closed += (s, e) => win.Dispatcher.InvokeShutdown();

                tsc.SetResult(win);
                Dispatcher.Run();
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();

            return tsc.Task.Result;

        }


        public void Write(string text, Brush foreground = null, int length = 0)
        {
            foreground = foreground ?? Brushes.White;

            if (length > 0)
            {
                var format = "{0, -" + length  + "}";
                text = string.Format(format, text);
            }

            txt.Inlines.Add(new Run
            {
                Text = text, 
                Foreground = foreground
            });
        }

        public void WriteLine(string text, string prefix = "")
        {
            Write(DateTime.Now.ToString("mm:ss:ffff"), Brushes.Lime, 11);

            if (prefix.HasAnyText())
                Write(prefix, Brushes.Yellow, 20);

            Write(text);
            txt.Inlines.Add(new LineBreak());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            txt.Text = "";
        }
    }
}
