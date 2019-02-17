using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MvvmKit
{
    public static class Exec
    {
        public static bool IsDesignTime
        {
            get
            {
                var designTimeEvaluator = new DependencyObject();

                return DesignerProperties.GetIsInDesignMode(designTimeEvaluator);
            }
        }

        public static bool IsRunTime
        {
            get
            {
                return !IsDesignTime;
            }
        }

        public static bool IsOnUiThread()
        {
            var dispatcher = Dispatcher.FromThread(Thread.CurrentThread);

            var res = (System.Windows.Application.Current != null) && 
                (dispatcher == System.Windows.Application.Current.Dispatcher);

            return res;
        }

        public static async Task RunOnUi(Action a)
        {
            await Application.Current.Dispatcher.InvokeAsync(a);
        }
    }
}
