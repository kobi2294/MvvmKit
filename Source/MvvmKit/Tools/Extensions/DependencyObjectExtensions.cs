using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MvvmKit
{
    public static class DependencyObjectExtensions
    {
        public static DependencyProperty GetDependencyPropertyByName(this DependencyObject obj, string name)
        {
            var descriptor = DependencyPropertyDescriptor.FromName(name, obj.GetType(), obj.GetType());
            return descriptor.DependencyProperty;
        }
    }
}
