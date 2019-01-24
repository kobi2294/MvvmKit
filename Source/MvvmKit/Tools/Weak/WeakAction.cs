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

        public WeakAction(object lifetimeController, object methodTarget, MethodInfo methodInfo)
            : base(lifetimeController, methodTarget, methodInfo)
        {
        }

        public WeakAction(object lifetimeController, Action action)
            : base(lifetimeController, action.Target, action.GetMethodInfo())
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
