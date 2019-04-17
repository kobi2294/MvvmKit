using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.Disposables
{
    public class OneTimeCalc : BaseDisposableWithData<(int a, int b)>
    {
        public OneTimeCalc(int a, int b)
            :base((a, b)) {}

        public int Calculate()
        {
            Validate();
            Dispose();
            return Data.a + Data.b;
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            Console.WriteLine("Disposing Calculator");
        }
    }
}
