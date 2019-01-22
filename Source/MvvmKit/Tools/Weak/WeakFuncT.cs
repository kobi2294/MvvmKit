using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class WeakFunc<T, TResult> : WeakFunc<TResult>, IWeakFunc, IWeakFunc<TResult>
    {
        private Func<object, T, TResult> _executer = null;

        public WeakFunc(object target, object actionTarget, MethodInfo methodInfo)
            : base(target, actionTarget, methodInfo)
        {
        }

        public WeakFunc(object target, Func<T, TResult> action)
            : base(target, action.Target, action.GetMethodInfo())
        {
        }

        public new TResult Execute()
        {
            return Execute(default(T));
        }

        public TResult Execute(T param)
        {
            if (IsAlive)
            {
                if (_executer == null) _executer = FastCaller.GetFunc<object, T, TResult>(_methodInfo);

                return _executer(Target, param);
            }
            else
            {
                MarkForDeletion();
                return default(TResult);
            }
        }

        public object ExecuteWithObject(object parameter)
        {
            var param = (T)parameter;
            return Execute(param);
        }

        TResult IWeakFunc<TResult>.ExecuteWithObject(object parameter)
        {
            var param = (T)parameter;
            return Execute(param);
        }
    }
}
