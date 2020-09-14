using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Reflection;

namespace MvvmKit
{
    public static class EnsureManager
    {
        private static IDictionary<MethodInfo, Type> _ensureMethods;

        public static bool IsHistoryEnabled { get; set; }

        private static List<EnsureSessionHistory> _history;

        private static BehaviorSubject<ImmutableList<EnsureSessionHistory>> _historySubject;

        static EnsureManager()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _ensureMethods = assemblies.SelectMany(asm => asm.GetTypes())
                                        .SelectMany(type => type.GetMethods())
                                        .Where(method => IsEnsureMethod(method))
                                        .ToDictionary(method => method, method => method.ReturnType);
            _history = new List<EnsureSessionHistory>();
            _historySubject = new BehaviorSubject<ImmutableList<EnsureSessionHistory>>(ImmutableList<EnsureSessionHistory>.Empty);
            IsHistoryEnabled = false;
        }

        public static bool IsEnsureMethod(MethodInfo method)
        {
            // 1. has to be static
            // 2. has to be decorated with [Ensure]
            if ((!method.IsStatic) || (!method.HasAttribute<EnsureAttribute>())) return false;

            // 3. has to have at least one parameter
            var parameters = method.GetParameters();
            if (parameters.Length == 0) return false;

            // 4. all parameters must either be immutables or ImmutableList<T> where T is immutable
            var allParametersAreImmutables = parameters
                .Select(prm => prm.ParameterType)
                .All(type => type.IsImmutableType() || type.IsImmutableListOfImmutables());

            if (!allParametersAreImmutables) return false;

            // 5. the return type must match the type of the first parameter
            var firstParamType = parameters.First().ParameterType;
            var returnType = method.ReturnType;
            if (firstParamType != returnType) return false;

            return true;
        }

        private static IEnumerable<MethodInfo> _ensureMethodsForType(Type type)
        {
            return _ensureMethods
                .Where(pair => type.IsInheritedFrom(pair.Value))
                .Select(pair => pair.Key);
        }

        public static IEnumerable<(EnsureContext context, MethodInfo method)> _createPlan(EnsureContext rootContext)
        {
            var allContexts = rootContext.FlattenSubContexts();

            return allContexts.SelectMany(ctxt => _ensureMethodsForType(ctxt.Entity.GetType())
                                                        .Select(method => (context: ctxt, method: method)));
        }

        public static object _callMethod(MethodInfo method, EnsureContext context)
        {
            var parameters = method
                .GetParameters()
                .Select(prm => prm.ParameterType)
                .Select(type => context.EntityOfType(type))
                .ToArray();

            var result = method.Invoke(null, parameters);
            return result;
        }

        public static T _runCycle<T>(T source, List<EnsureHistoryItem> history)
            where T: class, IImmutable
        {
            var rootContext = new EnsureContext(source);

            var plan = _createPlan(rootContext);

            foreach (var step in plan)
            {
                var res = _callMethod(step.method, step.context);
                if (!ReferenceEquals(step.context.Entity, res))
                {
                    // Ensure made changes, so the cycle needs to restart
                    var changedSource = step.context.ApplyNewValue(res) as T;

                    if (history != null)
                    {
                        var item = new EnsureHistoryItem(
                            ensureMethod: step.method.Name, 
                            before: source, 
                            after: changedSource, 
                            context: step.context
                            );
                        history.Add(item);
                    }
                    return changedSource;
                }
            }

            return source;
        }

        public static IObservable<ImmutableList<EnsureSessionHistory>> GetHistory()
        {
            return _historySubject.AsObservable();
        }

        public static T Ensure<T>(this T source, object action = null)
            where T: class, IImmutable
        {
            var current = source;
            bool success;

            var history = IsHistoryEnabled ? new List<EnsureHistoryItem>() : null;

            do
            {
                var next = _runCycle(current, history);
                success = ReferenceEquals(next, current);
                current = next;
            } while (!success);

            if (IsHistoryEnabled)
            {
                var record = new EnsureSessionHistory(
                    action: action,
                    before: source,
                    after: current,
                    items: history.ToImmutableList());
                _history.Add(record);
                _historySubject.OnNext(_history.ToImmutableList());
            }

            return current;
        }

    }
}
