using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class TupleExtensions
    {
        public static IEnumerable<object> Enumerate(this ValueType tpl)
        {
            var ivt = tpl as ITuple;
            if (ivt == null) yield break;

            for (int i = 0; i < ivt.Length; i++)
            {
                yield return ivt[i];
            }
        }
    }
}
