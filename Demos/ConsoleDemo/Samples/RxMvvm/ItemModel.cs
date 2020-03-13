using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.RxMvvm
{
    public class ItemModel: IImmutable
    {
        public string Uid { get; }

        public string DisplayName { get; }

        public string Category { get; }

        public ItemModel(string uid, string displayName, string category)
        {
            Uid = uid;
            DisplayName = displayName;
            Category = category;
        }
    }
}
