using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class RegionBehavior
    {
        internal Task BeforeNavigate()
        {
            return OnBeforeNavigate();
        }

        protected virtual Task OnBeforeNavigate()
        {
            return Tasks.Empty;
        }

        internal Task AfterNavigate()
        {
            return AfterNavigate();
        }

        protected virtual Task OnAfterNavigate()
        {
            return Tasks.Empty;
        }

        internal Task BeforeClear()
        {
            return OnBeforeClear();
        }

        protected virtual Task OnBeforeClear()
        {
            return Tasks.Empty;
        }

        internal Task AfterClear()
        {
            return Tasks.Empty;
        }

        protected virtual Task OnAfterClear()
        {
            return Tasks.Empty;
        }
    }
}
