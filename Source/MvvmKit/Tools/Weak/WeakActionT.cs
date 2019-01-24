using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class WeakAction<T> : WeakAction, IWeakActionWithParam1
    {
        private Action<object, T> _executer = null;

        public WeakAction(object lifetimeController, object actionTarget, MethodInfo methodInfo)
            : base(lifetimeController, actionTarget, methodInfo)
        {
        }

        public WeakAction(object lifetimeController, Action<T> action)
            : base(lifetimeController, action.Target, action.GetMethodInfo())
        {
        }

        public new void Execute()
        {
            Execute(default(T));
        }

        public void Execute(T param)
        {
            if (IsAlive)
            {
                if (_executer == null) _executer = FastCaller.GetAction<object, T>(_methodInfo);

                _executer(Target, param);
            }
            else
            {
                MarkForDeletion();
            }
        }

        public void ExecuteWithObject(object parameter)
        {
            var param = (T)parameter;
            Execute(param);
        }
    }

    public class WeakAction<T1, T2> : WeakAction, IWeakActionWithParam2
    {
        private Action<object, T1, T2> _executer = null;

        public WeakAction(object target, object actionTarget, MethodInfo methodInfo)
            : base(target, actionTarget, methodInfo)
        {
        }

        public WeakAction(object target, Action<T1, T2> action)
            : base(target, action.Target, action.GetMethodInfo())
        {
        }

        public new void Execute()
        {
            Execute(default(T1), default(T2));
        }

        public void Execute(T1 param)
        {
            Execute(param, default(T2));
        }

        public void Execute(T1 param1, T2 param2)
        {
            if (IsAlive)
            {
                if (_executer == null) _executer = FastCaller.GetAction<object, T1, T2>(_methodInfo);

                _executer(Target, param1, param2);
            }
            else
            {
                MarkForDeletion();
            }
        }

        public void ExecuteWithObject(object parameter)
        {
            var param = (T1)parameter;
            Execute(param);
        }


        public void ExecuteWithObject(object param1, object param2)
        {
            var p1 = (T1)param1;
            var p2 = (T2)param2;
            Execute(p1, p2);
        }
    }
}
