using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MvvmKit
{
    internal class RegionServiceBindable: BindableBase
    {
        private ComponentBase _ViewModel;
        public ComponentBase ViewModel { get { return _ViewModel; } set { SetProperty(ref _ViewModel, value); } }

        private DataTemplateSelector _ViewSelector;
        public DataTemplateSelector ViewSelector { get { return _ViewSelector; } set { SetProperty(ref _ViewSelector, value); } }
    }
}
