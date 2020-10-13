using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MvvmKit
{
    public class ComboBoxEx: ComboBox
    {
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            BindingExpression be = GetBindingExpression(SelectedValueProperty);
            be?.UpdateTarget();

        }
    }
}
