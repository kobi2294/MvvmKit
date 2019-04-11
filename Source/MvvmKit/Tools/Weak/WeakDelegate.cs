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
        protected WeakReference _lifetimeController;
        protected WeakReference _delegateTargetReference;
        private bool _isTargetClosure = false;
        private object _strongReferenceToClosure;
        private ConditionalWeakTable<object, object> _conditionalTable; // to be used only for closures

        private static bool _isClosure(object target)
        {
            var typ = target.GetType();
            var typInfo = typ.GetTypeInfo();
            var isInvisible = !typInfo.IsVisible;

            var isCompilerGenerated = typInfo.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Count() > 0;
            var isNested = typ.IsNested;

            return isNested && isCompilerGenerated && isInvisible;
        }

        private void _handleClosures(object lifetimeController, object methodTarget)
        {
            if ((methodTarget != null) && (_isClosure(methodTarget)))
            {
                // closures are a special case. Their target is an instance of a class that is generated automatically, and there may not be a refernce to them at all,
                // except the one that the delegate is holding. In that case, the garbage collector may collect them instantly. We would prefer that their lifetime
                // will be decided by the instance of the class inside which they were defined. But we may not have it.
                // They may be holding a reference to their parent, so we make several attempts to grab a hold of a parent instance and tie 
                // the closure instance to it.

                // if we have a supplied lifetime controller, we will tie the closure to it, 
                // otherwise, we try to find a field inside the closure that points to the right class
                _isTargetClosure = true;

                if (lifetimeController == null)
                {
                    var typ = methodTarget.GetType();
                    var fieldToParent = typ.GetRuntimeFields().Where(f => f.FieldType == typ.DeclaringType).LastOrDefault();

                    if (fieldToParent != null)
                    {
                        lifetimeController = fieldToParent.GetValue(methodTarget);
                    }
                }

                if (lifetimeController == null)
                {
                    // if there is absolutely no target to rely on, we make a strong reference to the action target, even though this WILL lead to memory leak, 
                    // we just hope its a small one since its a "PURE" closure.

                    _strongReferenceToClosure = methodTarget;
                }
                else
                {
                    // since we have a target, we dont need to hold a close reference to the closure, we can save it from instantly dying by 
                    // creating a garbage collection conditional weak table, that associates the method target (the closure) with the lifetime controller
                    if (_lifetimeController == null) _lifetimeController = new WeakReference(lifetimeController);

                    _conditionalTable = new ConditionalWeakTable<object, object>();
                    _conditionalTable.Add(lifetimeController, methodTarget);
                }
            }

        }

        protected WeakDelegate(object lifetimeController, object methodTarget, MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;

            if (!_methodInfo.IsStatic)
            {
                if (lifetimeController != null)
                {
                    _lifetimeController = new WeakReference(lifetimeController);
                }

                _delegateTargetReference = new WeakReference(methodTarget);
            }

            _handleClosures(lifetimeController, methodTarget);
        }

        public MethodInfo Method
        {
            get { return _methodInfo; }
        }

        public string MethodName
        {
            get { return _methodInfo.Name; }
        }

        public bool IsStatic
        {
            get { return _methodInfo.IsStatic; }
        }

        public bool IsClosure
        {
            get { return _isTargetClosure; }
        }

        public object Target
        {
            get { return (IsStatic) ? null : _delegateTargetReference.Target; }
        }

        public object Owner
        {
            get { return _lifetimeController.Target; }
        }

        public bool IsAlive
        {
            get
            {
                var isTargetAlive = ((_lifetimeController == null) || (_lifetimeController.IsAlive));

                return IsStatic || (isTargetAlive && _delegateTargetReference.IsAlive);
            }
        }

        public void MarkForDeletion()
        {
            _conditionalTable = null;
            _strongReferenceToClosure = null;
        }
    }
}
