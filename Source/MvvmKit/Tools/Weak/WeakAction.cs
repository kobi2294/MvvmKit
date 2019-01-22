using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public class WeakAction : WeakDelegate
    {
        private Action<object> _executer = null;

        public WeakAction(object target, object actionTarget, MethodInfo methodInfo)
            : base(target, actionTarget, methodInfo)
        {
        }

        public WeakAction(object target, Action action)
            : base(target, action.Target, action.GetMethodInfo())
        {
        }

        public void Execute()
        {
            if (IsAlive)
            {
                if (_executer == null) _executer = FastCaller.GetAction<object>(_methodInfo);
                _executer(Target);
            }
            else
            {
                MarkForDeletion();
            }
        }
    }
}
