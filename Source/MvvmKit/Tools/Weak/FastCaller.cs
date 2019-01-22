using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class FastCaller
    {
        static readonly Dictionary<Tuple<MethodInfo, Type>, object> _actions = new Dictionary<Tuple<MethodInfo, Type>, object>();

        #region Delegate Generation

        private static DelegateType _getDelegate<DelegateType>(MethodInfo method)
        {
            lock (_actions)
            {
                object a;
                var key = Tuple.Create(method, typeof(DelegateType));

                if (_actions.TryGetValue(key, out a))
                    return (DelegateType)a;

                var res = _createDelegateByExpression<DelegateType>(method);

                _actions[key] = res;
                return res;
            }
        }

        private static MethodInfo _methodInfoFromDelegateType(Type delegateType)
        {
            return delegateType.GetRuntimeMethods().First(mi => mi.Name == "Invoke");
        }

        private static DelegateType _createDelegateByExpression<DelegateType>(MethodInfo method)
        {
            MethodInfo delegateInfo = _methodInfoFromDelegateType(typeof(DelegateType));

            var delegateParameterInfos = delegateInfo.GetParameters().ToArray();

            ParameterExpression[] delegateParameterExpressions = delegateInfo.GetParameters()
                                                         .Select(m => Expression.Parameter(m.ParameterType, m.Name))
                                                         .ToArray();

            Type[] methodParamTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

            Expression[] argumentExpressions = delegateParameterExpressions
                .Skip(1)
                .Zip(methodParamTypes, (param, typ) => (param.Type == typ) ? (Expression)param : (Expression)Expression.Convert(param, typ))
                .ToArray();

            Expression instanceExp = null;

            if (!method.IsStatic)
            {
                instanceExp = delegateParameterExpressions[0];
                if (instanceExp.Type != method.DeclaringType)
                {
                    instanceExp = (Expression)Expression.Convert(instanceExp, method.DeclaringType);
                }
            }


            Expression methodCall = Expression.Call(instanceExp, method, argumentExpressions);

            // check if result type requires conversion
            if (method.ReturnType != delegateInfo.ReturnType)
            {
                methodCall = Expression.Convert(methodCall, delegateInfo.ReturnType);
            }

            var res = Expression.Lambda<DelegateType>(methodCall, delegateParameterExpressions)
                                .Compile();

            return res;
        }

        #endregion

        public static Action<T> GetAction<T>(MethodInfo method)
        {
            return _getDelegate<Action<T>>(method);
        }

        public static Action<T, Arg1> GetAction<T, Arg1>(MethodInfo method)
        {
            return _getDelegate<Action<T, Arg1>>(method);
        }

        public static Action<T, Arg1, Arg2> GetAction<T, Arg1, Arg2>(MethodInfo method)
        {
            return _getDelegate<Action<T, Arg1, Arg2>>(method);
        }

        public static Action<T, Arg1, Arg2, Arg3> GetAction<T, Arg1, Arg2, Arg3>(MethodInfo method)
        {
            return _getDelegate<Action<T, Arg1, Arg2, Arg3>>(method);
        }

        public static Action<T, Arg1, Arg2, Arg3, Arg4> GetAction<T, Arg1, Arg2, Arg3, Arg4>(MethodInfo method)
        {
            return _getDelegate<Action<T, Arg1, Arg2, Arg3, Arg4>>(method);
        }

        public static Func<T, TRes> GetFunc<T, TRes>(MethodInfo method)
        {
            return _getDelegate<Func<T, TRes>>(method);
        }

        public static Func<T, Arg1, TRes> GetFunc<T, Arg1, TRes>(MethodInfo method)
        {
            return _getDelegate<Func<T, Arg1, TRes>>(method);
        }

        public static Func<T, Arg1, Arg2, TRes> GetFunc<T, Arg1, Arg2, TRes>(MethodInfo method)
        {
            return _getDelegate<Func<T, Arg1, Arg2, TRes>>(method);
        }

        public static Func<T, Arg1, Arg2, Arg3, TRes> GetFunc<T, Arg1, Arg2, Arg3, TRes>(MethodInfo method)
        {
            return _getDelegate<Func<T, Arg1, Arg2, Arg3, TRes>>(method);
        }
    }
}
