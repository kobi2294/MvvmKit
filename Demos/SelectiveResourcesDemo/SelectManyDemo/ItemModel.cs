using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectiveResourcesDemo.SelectManyDemo
{
    public class ItemModel: IImmutable
    {
        public string Uid { get; }

        public string DisplayName { get; }

        public ItemModel(
            string uid = "", 
            string displayName = "")
        {
            Uid = uid;
            DisplayName = displayName;
        }
    }
}
