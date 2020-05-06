using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ConsoleDemo.Samples.ToGetter
{
    public static class Main
    {
        public static void Run()
        {
            var item = new Item
            {
                Uid = "uid",
                DisplayName = "display name"
            };

            var propname = nameof(item.Uid);

            var path = new PropertyPath("Uid.Length");
            var key2 = path.Evaluate(item);


            // one way...
            var prop = item.GetType().GetProperty(propname);

            var getter = prop.ToGetter<object, object>();
            var key = getter(item);


        }
    }
}
