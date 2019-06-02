using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKitAppSample.Model
{
    public class TodoItem
    {
        public string Uid { get; set; }

        public string Caption { get; set; }

        public bool IsChecked { get; set; }
    }
}
