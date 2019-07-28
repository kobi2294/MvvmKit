using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class AsyncLazyInit
    {
        private readonly object _mutex = new object();
        private readonly Func<Task> _initFunc;
        private Lazy<Task> _lazy;

        public AsyncLazyInit(Func<Task> initFunc)
        {
            if (initFunc == null)
                throw new ArgumentNullException(nameof(initFunc));

            _initFunc = initFunc;
            _lazy = new Lazy<Task>(_initFunc);
        }

        public Task Task
        {
            get
            {
                lock (_mutex)
                    return _lazy.Value;
            }
        }

    }
}
