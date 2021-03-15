using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Reflection;
using System.Collections.Concurrent;

namespace MvvmKit
{
    public static class EnsureManager
    {
        private class ConditionEntry
        {
            public MethodInfo Ensurer { get; set; }

            public Func<EnsureContext, bool> Lambda { get; set; }

            public string ConditionName { get; set; }

            public bool ExpectedValue { get; set; }
        }

        private static ConcurrentDictionary<MethodInfo, Type> _ensureMethods;
        private static ConcurrentDictionary<MethodInfo, Func<EnsureContext, object>> _ensureLambdas;

        private static ConcurrentDictionary<string, MethodInfo> _conditionMethodByName;
        private static ConcurrentDictionary<string, Func<EnsureContext, bool>> _conditionLambdasByName;

        private static ConcurrentDictionary<Type, MethodInfo[]> _ensureMethodsPerType;

        private static ILookup<MethodInfo, ConditionEntry> _conditionsPerEnsurer;


        public static bool IsHistoryEnabled { get; set; }

        private static List<EnsureSessionHistory> _history;

        private static BehaviorSubject<ImmutableList<EnsureSessionHistory>> _historySubject;

        static EnsureManager()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _ensureMethods = assemblies.SelectMany(asm => asm.GetTypes())
                                        .SelectMany(type => type.GetMethods())
                                        .Where(method => IsEnsureMethod(method))
                                        .ToConcurrentDictionary(method => method, method => method.ReturnType);

            _ensureMethodsPerType = new ConcurrentDictionary<Type, MethodInfo[]>();

            _ensureLambdas = _ensureMethods.Keys
                .Select(method =>
                {
                    var argumentEnumerator = ArgumentEnumerators.ForFunc<EnsureContext>(method, (ctxt, type) => ctxt.EntityOfType(type));
                    return (method: method, lambda: method.CompileTo<Func<EnsureContext, object>>(argumentEnumerator));
                })
                .ToConcurrentDictionary(pair => pair.method, pair => pair.lambda);

            _conditionMethodByName = assemblies
                .SelectMany(asm => asm.GetTypes())
                .SelectMany(type => type.GetMethods())
                .Where(method => IsEnsureCondition(method))
                .ToConcurrentDictionary(method => method.Name);


            _conditionLambdasByName = _conditionMethodByName.Values
                .Select(method =>
                {
                    var argumentEnumerator = ArgumentEnumerators.ForFunc<EnsureContext>(method, (ctxt, type) => ctxt.EntityOfType(type));
                    return (method: method, lambda: method.CompileTo<Func<EnsureContext, bool>>(argumentEnumerator));
                })
                .ToConcurrentDictionary(pair => pair.method.Name, pair => pair.lambda);

            _conditionsPerEnsurer = _ensureMethods.Keys
                .SelectMany(method => method.GetCustomAttributes<EnsureIfAttribute>().Select(attrib => (method, attrib)))
                .Select(pair => new ConditionEntry
                {
                    Ensurer = pair.method,
                    ExpectedValue = pair.attrib.Value,
                    ConditionName = pair.attrib.MethodName,
                    Lambda = _conditionLambdasByName[pair.attrib.MethodName]
                })
                .ToLookup(entry => entry.Ensurer);

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

        public static bool IsEnsureCondition(MethodInfo method)
        {
            // 1. has to be static
            // 2. has to be decorated with [EnsureCondition]
            if ((!method.IsStatic) || (!method.HasAttribute<EnsureConditionAttribute>())) return false;


            // 3. all parameters must either be immutables or ImmutableList<T> where T is immutable
            var parameters = method.GetParameters();
            var allParametersAreImmutables = parameters
                .Select(prm => prm.ParameterType)
                .All(type => type.IsImmutableType() || type.IsImmutableListOfImmutables());

            if (!allParametersAreImmutables) return false;

            // 4. the return type must be boolean
            if (method.ReturnType != typeof(bool)) return false;

            return true;
        }

        private static IEnumerable<MethodInfo> _ensureMethodsForType(Type type)
        {
            var res = _ensureMethodsPerType.GetOrAdd(type, t =>
            _ensureMethods
                .Where(pair => t.IsInheritedFrom(pair.Value))
                .OrderBy(pair => pair.Key.MetadataToken)
                .Select(pair => pair.Key)
                .ToArray()
                );

            return res;
        }

        public static IEnumerable<(int index, EnsureContext context, MethodInfo method)> _createPlan(EnsureContext rootContext)
        {
            var allContexts = rootContext.FlattenSubContexts();

            return allContexts
                .SelectMany(ctxt => _ensureMethodsForType(ctxt.Entity.GetType())
                                                        .Select(method => (context: ctxt, method: method)))
                .Select((pair, index) => (index: index, context: pair.context, method: pair.method));
        }

        public static object _callMethod(MethodInfo method, EnsureContext context)
        {
            // the method should only be called if the conditions apply, so first we allow each condition to run and prevent calling the ensurer
            var conditions = _conditionsPerEnsurer[method];

            foreach (var condition in conditions)
            {
                var res = condition.Lambda(context);
                if (res != condition.ExpectedValue)
                    return context.Entity;  // return the original entity of he context
            }

            // if we have got here, all conditions apply, so we can run the ensurer itself
            var lambda = _ensureLambdas[method];
            var result = lambda(context);

            return result;
        }

        public static T _runCycle<T>(T source, List<EnsureHistoryItem> history)
            where T : class, IImmutable
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

        public static T Ensure<T>(this T source, object action)
            where T : class, IImmutable
        {
            var current = source;
            bool success;
            int counter = 0;

            var history = IsHistoryEnabled ? new List<EnsureHistoryItem>() : null;

            do
            {
                var next = _runCycle(current, history);
                success = ReferenceEquals(next, current);
                current = next;
                counter++;

                if (counter > 100)
                {
                    Debug.WriteLine("ENSURE WARNING, infinite loop suspected");
                }
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
