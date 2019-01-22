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

        public WeakFunc(object target, object actionTarget, MethodInfo methodInfo)
            : base(target, actionTarget, methodInfo)
        {
        }

        public WeakFunc(object target, Func<TResult> action)
            : base(target, action.Target, action.GetMethodInfo())
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
