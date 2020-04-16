using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Mvvm.Rx.StoreHistory
{
    public class HistoryRecord
    {
        public DateTime Date { get; set; }
        public object Action { get; set; }

        public object PreviousState { get; set; }

        public object NextState { get; set; }
    }
}
