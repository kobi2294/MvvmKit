using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.LinqProvider
{
    public class MyType<T> 
    {
        private List<string> _queries = new List<string>();

        public void Add(string str)
        {
            _queries.Add(str);
        }

        public MyType<K> To<K>()
        {
            var res = new MyType<K>();
            res._queries = _queries.ToList();
            return res;
        }

        public override string ToString()
        {
            return string.Join("\n", _queries);
        }
    }

   
}
