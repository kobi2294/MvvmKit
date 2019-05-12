using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MvvmKit
{
    public class ViewTemplateSelector : DataTemplateSelector
    {
        public IViewResolver ViewResolver { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate res = null;

            if ((ViewResolver != null) &&
                (item != null))
            {
                var vmType = item.GetType();
                var vType = ViewResolver.GetViewType(vmType);

                if (vType != null)
                    res = DataTemplateWrapper.ForViewModel(vmType, vType);
            }

            return res ?? base.SelectTemplate(item, container);
        }
    }
}
