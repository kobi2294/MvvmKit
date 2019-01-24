using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class WeakFunc<TResult> : WeakDelegate
    {
        private Func<object, TResult> _executer = null;

        public WeakFunc(object lifetimeController, object actionTarget, MethodInfo methodInfo)
            : base(lifetimeController, actionTarget, methodInfo)
        {
        }

        public WeakFunc(object lifetimeController, Func<TResult> action)
            : base(lifetimeController, action.Target, action.GetMethodInfo())
        {
        }

        public TResult Execute()
        {
            if (IsAlive)
            {
                if (_executer == null) _executer = FastCaller.GetFunc<object, TResult>(_methodInfo);

                return _executer(Target);
            }
            else
            {
                MarkForDeletion();
                return default(TResult);
            }
        }
    }
}
