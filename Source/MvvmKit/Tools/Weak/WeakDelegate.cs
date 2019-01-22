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
        protected WeakReference _targetReference;
        protected WeakReference _actionTargetReference;
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

        private void _handleClosures(object target, object actionTarget)
        {
            if ((actionTarget != null) && (_isClosure(actionTarget)))
            {
                // closures are a special case. They may be holding a reference to their parent, and at the same time there may not be a reference to them
                // we make several attempts to grab a hold of a parent instance and tie the closure instance to it.

                // if we have a supplied target, we will tie the closure to it, otherwise, we try to find a field inside the closure that points to the right class
                _isTargetClosure = true;

                if (target == null)
                {
                    var typ = actionTarget.GetType();
                    var fieldToParent = typ.GetRuntimeFields().Where(f => f.FieldType == typ.DeclaringType).LastOrDefault();

                    if (fieldToParent != null)
                    {
                        target = fieldToParent.GetValue(actionTarget);
                    }
                }

                if (target == null)
                {
                    // if there is absolutely no target to rely on, we make a strong reference to the action target, even though this WILL lead to memory leak, 
                    // we just hope its a small one since its a "PURE" closure.

                    _strongReferenceToClosure = actionTarget;
                }
                else
                {
                    // since we have a target, we dont need to hold a close reference to the closure, we can save it from instantly dying by create a garbage collection
                    // conditional weak table, that associates the target with the closure
                    if (_targetReference == null) _targetReference = new WeakReference(target);

                    _conditionalTable = new ConditionalWeakTable<object, object>();
                    _conditionalTable.Add(target, actionTarget);
                }
            }

        }

        protected WeakDelegate(object target, object actionTarget, MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;

            if (!_methodInfo.IsStatic)
            {
                if (target != null)
                {
                    _targetReference = new WeakReference(target);
                }

                _actionTargetReference = new WeakReference(actionTarget);
            }

            _handleClosures(target, actionTarget);
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
            get { return (IsStatic) ? null : _actionTargetReference.Target; }
        }

        public bool IsAlive
        {
            get
            {
                var isTargetAlive = ((_targetReference == null) || (_targetReference.IsAlive));

                return IsStatic || (isTargetAlive && _actionTargetReference.IsAlive);
            }
        }

        public void MarkForDeletion()
        {
            _conditionalTable = null;
            _strongReferenceToClosure = null;
        }
    }
}
