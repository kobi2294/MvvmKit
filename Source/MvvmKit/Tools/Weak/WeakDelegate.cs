using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public abstract class WeakDelegate
    {
        protected MethodInfo _methodInfo;
        protected WeakReference _lifetimeController = null;
        protected WeakReference _methodTarget = null;
        private ConditionalWeakTable<object, object> _conditionalTable; // to be used only for closures

        protected WeakDelegate(object lifetimeController, object methodTarget, MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));

            if (lifetimeController == null)
                throw new ArgumentNullException(nameof(lifetimeController));

            _methodInfo = methodInfo;

            _lifetimeController = new WeakReference(lifetimeController);
            if (methodTarget != null) _methodTarget = new WeakReference(methodTarget);

            // since we do not hold a direct reference to the method target, it may be garbage collected before the lifetime controller
            // "dies". To avoid it, we create a conditional weak table that keeps the method target alive as long as the lifetime controller is alive

            // this is specifically important for closures, where the compiler creates a temporary instance that closes the local variables, 
            // and the only object to point to it is the delegate. since we replace the delegate with a "weak" delegate - it now does not really 
            // point to the temprary object, which means that it will be instantly collected. So  the lifetime controller - keeps it alive.
            _conditionalTable = new ConditionalWeakTable<object, object>();
            _conditionalTable.Add(lifetimeController, methodTarget);
        }

        public MethodInfo Method => _methodInfo;

        public string MethodName => _methodInfo.Name;

        public bool IsStatic => _methodInfo.IsStatic;

        public object Target => _methodTarget?.Target;

        public object Owner => _lifetimeController.Target;

        public bool IsAlive => _lifetimeController.IsAlive;

        public void MarkForDeletion()
        {
            if (_conditionalTable != null)
                _conditionalTable.Remove(_lifetimeController);
            _conditionalTable = null;
        }
    }
}
