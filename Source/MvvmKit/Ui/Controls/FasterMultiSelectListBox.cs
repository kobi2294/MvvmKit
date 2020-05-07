using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MvvmKit
{
    public class FasterMultiSelectListBox : ListBox
    {
        public new void SetSelectedItems(IEnumerable items)
        {
            base.SetSelectedItems(items);
        }
    }
}
