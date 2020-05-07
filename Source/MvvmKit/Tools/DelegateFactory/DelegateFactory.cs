using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MvvmKit
{
    public static class DelegateFactory
    {
        static readonly Dictionary<(MethodInfo method, Type delegateType), object> _actions 
            = new Dictionary<(MethodInfo method, Type delegateType), object>();

        static readonly Dictionary<(FieldInfo field, Type delegateType), object> _fieldGetters
            = new Dictionary<(FieldInfo field, Type delegateType), object>();

        static readonly Dictionary<(FieldInfo field, Type delegateType), object> _fieldSetters
            = new Dictionary<(FieldInfo field, Type delegateType), object>();

        #region Delegate Generation

        private static DelegateType _getDelegate<DelegateType>(MethodInfo method)
        {
            lock (_actions)
            {
                object a;
                var key = (method: method, delegateType: typeof(DelegateType));

                if (_actions.TryGetValue(key, out a))
                    return (DelegateType)a;

                var res = _createDelegateByExpression<DelegateType>(method);

                _actions[key] = res;
                return res;
            }
        }

        private static Func<TEntity, TField> _getFieldGetter<TEntity, TField>(FieldInfo field)
        {
            lock (_fieldGetters)
            {
                object a;
                var key = (field: field, delegateType: typeof(Func<TEntity, TField>));

                if (_fieldGetters.TryGetValue(key, out a))
                    return (Func<TEntity, TField>)a;

                var res = _createFieldGetterByExpression<TEntity, TField>(field);

                _fieldGetters[key] = res;
                return res;
            }
        }

        private static Action<TEntity, TField> _getFieldSetter<TEntity, TField>(FieldInfo field)
        {
            lock (_fieldSetters)
            {
                object a;
                var key = (field: field, delegateType: typeof(Action<TEntity, TField>));

                if (_fieldSetters.TryGetValue(key, out a))
                    return (Action<TEntity, TField>)a;

                var res = _createFieldSetterByExpression<TEntity, TField>(field);

                _fieldSetters[key] = res;
                return res;
            }
        }

        private static MethodInfo _methodInfoFromDelegateType(Type delegateType)
        {
            return delegateType.GetRuntimeMethods().First(mi => mi.Name == "Invoke");
        }

        public static DelegateType _createDelegateByExpression<DelegateType>(MethodInfo method, params object[] missingParamValues)
        {
            var queueMissingParams = new Queue<object>(missingParamValues);

            var dgtMi = typeof(DelegateType).GetMethod("Invoke");
            var dgtRet = dgtMi.ReturnType;
            var dgtParams = dgtMi.GetParameters();

            var paramsOfDelegate = dgtParams
                .Select(tp => Expression.Parameter(tp.ParameterType, tp.Name))
                .ToArray();

            var methodParams = method.GetParameters();

            if (method.IsStatic)
            {
                var paramsToPass = methodParams
                    .Select((p, i) => CreateParam(paramsOfDelegate, i, p, queueMissingParams))
                    .ToArray();

                var expr = Expression.Lambda<DelegateType>(
                    Expression.Call(method, paramsToPass),
                    paramsOfDelegate);

                return expr.Compile();
            }
            else
            {
                var paramThis = Expression.Convert(paramsOfDelegate[0], method.DeclaringType);

                var paramsToPass = methodParams
                    .Select((p, i) => CreateParam(paramsOfDelegate, i + 1, p, queueMissingParams))
                    .ToArray();

                var expr = Expression.Lambda<DelegateType>(
                    Expression.Call(paramThis, method, paramsToPass),
                    paramsOfDelegate);

                return expr.Compile();
            }
        }

        private static Expression CreateParam(ParameterExpression[] paramsOfDelegate, int i, ParameterInfo callParamType, Queue<object> queueMissingParams)
        {
            if (i < paramsOfDelegate.Length)
                return Expression.Convert(paramsOfDelegate[i], callParamType.ParameterType);

            if (queueMissingParams.Count > 0)
                return Expression.Constant(queueMissingParams.Dequeue());

            if (callParamType.ParameterType.IsValueType)
                return Expression.Constant(Activator.CreateInstance(callParamType.ParameterType));

            return Expression.Constant(null);
        }

        //private static DelegateType _createDelegateByExpression<DelegateType>(MethodInfo method)
        //{
        //    MethodInfo delegateInfo = _methodInfoFromDelegateType(typeof(DelegateType));

        //    var delegateParameterInfos = delegateInfo.GetParameters().ToArray();

        //    ParameterExpression[] delegateParameterExpressions = delegateInfo.GetParameters()
        //                                                 .Select(m => Expression.Parameter(m.ParameterType, m.Name))
        //                                                 .ToArray();

        //    Type[] methodParamTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

        //    Expression[] argumentExpressions = delegateParameterExpressions
        //        .Skip(1)
        //        .Zip(methodParamTypes, (param, typ) => (param.Type == typ) ? (Expression)param : (Expression)Expression.Convert(param, typ))
        //        .ToArray();

        //    Expression instanceExp = null;

        //    if (!method.IsStatic)
        //    {
        //        instanceExp = delegateParameterExpressions[0];
        //        if (instanceExp.Type != method.DeclaringType)
        //        {
        //            instanceExp = (Expression)Expression.Convert(instanceExp, method.DeclaringType);
        //        }
        //    }


        //    Expression methodCall = Expression.Call(instanceExp, method, argumentExpressions);

        //    // check if result type requires conversion
        //    if (method.ReturnType != delegateInfo.ReturnType)
        //    {
        //        methodCall = Expression.Convert(methodCall, delegateInfo.ReturnType);
        //    }

        //    var res = Expression.Lambda<DelegateType>(methodCall, delegateParameterExpressions)
        //                        .Compile();

        //    return res;
        //}

        private static Func<TEntity, TField> _createFieldGetterByExpression<TEntity, TField>(FieldInfo field)
        {
            ParameterExpression targetExp = Expression.Parameter(typeof(TEntity), "target");

            Expression argExp = (typeof(TEntity) == field.ReflectedType)
                ? targetExp as Expression
                : Expression.Convert(targetExp, field.ReflectedType);

            MemberExpression fieldExp = Expression.Field(argExp, field);

            Expression resExp = (typeof(TField) == field.FieldType)
                ? fieldExp as Expression
                : Expression.Convert(fieldExp, typeof(TField));

            var getter = Expression.Lambda<Func<TEntity, TField>>(resExp, targetExp).Compile();
            return getter;
        }

        private static Action<TEntity, TField> _createFieldSetterByExpression<TEntity, TField>(FieldInfo field)
        {
            ParameterExpression targetExp = Expression.Parameter(typeof(TEntity), "target");
            Expression argExp = (typeof(TEntity) == field.ReflectedType)
                ? targetExp as Expression
                : Expression.Convert(targetExp, field.ReflectedType);

            ParameterExpression valueExp = Expression.Parameter(typeof(TField), "value");
            Expression resExp = (typeof(TField) == field.FieldType)
                ? valueExp as Expression
                : Expression.Convert(valueExp, field.FieldType);


            MemberExpression fieldExp = Expression.Field(argExp, field);
            BinaryExpression assignExp = Expression.Assign(fieldExp, resExp);

            var setter = Expression.Lambda<Action<TEntity, TField>>(assignExp, targetExp, valueExp).Compile();
            return setter;
        }

        private static Func<TEntity, TProp> _getGetter<TEntity, TProp>(PropertyInfo prop)
        {
            var setter = prop.GetGetMethod();
            return _getDelegate<Func<TEntity, TProp>>(setter);
        }

        private static Action<TEntity, TProp> _getSetter<TEntity, TProp>(PropertyInfo prop)
        {
            var setter = prop.GetSetMethod();
            return _getDelegate<Action<TEntity, TProp>>(setter);
        }

        private static Func<TEntity, TField> _getGetter<TEntity, TField>(FieldInfo field)
        {
            return _getFieldGetter<TEntity, TField>(field);
        }

        private static Action<TEntity, TField> _getSetter<TEntity, TField>(FieldInfo field)
        {
            return _getFieldSetter<TEntity, TField>(field);
        }

        #endregion

        public static DelegateType ToDelegate<DelegateType>(this MethodInfo method)
            where DelegateType : Delegate
        {
            return _getDelegate<DelegateType>(method);
        }

        #region Action Apis
        public static Action ToAction(this MethodInfo method)
        {
            return method.ToDelegate<Action>();
        }

        public static Action<T> ToAction<T>(this MethodInfo method)
        {
            return method.ToDelegate<Action<T>>();
        }

        public static Action<T1, T2> ToAction<T1, T2>(this MethodInfo method)
        {
            return method.ToDelegate<Action<T1, T2>>();
        }

        public static Action<T1, T2, T3> ToAction<T1, T2, T3>(this MethodInfo method)
        {
            return method.ToDelegate<Action<T1, T2, T3>>();
        }

        public static Action<T1, T2, T3, T4> ToAction<T1, T2, T3, T4>(this MethodInfo method)
        {
            return method.ToDelegate<Action<T1, T2, T3, T4>>();
        }

        #endregion

        #region Func Apis

        public static Func<T> ToFunc<T>(this MethodInfo method)
        {
            return method.ToDelegate<Func<T>>();
        }

        public static Func<Arg1, T> ToFunc<Arg1, T>(this MethodInfo method)
        {
            return method.ToDelegate<Func<Arg1, T>>();
        }

        public static Func<Arg1, Arg2, T> ToFunc<Arg1, Arg2, T>(this MethodInfo method)
        {
            return method.ToDelegate<Func<Arg1, Arg2, T>>();
        }

        public static Func<Arg1, Arg2, Arg3, T> ToFunc<Arg1, Arg2, Arg3, T>(this MethodInfo method)
        {
            return method.ToDelegate<Func<Arg1, Arg2, Arg3, T>>();
        }

        public static Func<Arg1, Arg2, Arg3, Arg4, T> ToFunc<Arg1, Arg2, Arg3, Arg4, T>(this MethodInfo method)
        {
            return method.ToDelegate<Func<Arg1, Arg2, Arg3, Arg4, T>>();
        }

        #endregion

        public static Func<TEntity, TProp> ToGetter<TEntity, TProp>(this PropertyInfo prop)
        {
            return _getGetter<TEntity, TProp>(prop);
        }

        public static Func<TEntity, TField> ToGetter<TEntity, TField>(this FieldInfo field)
        {
            return _getGetter<TEntity, TField>(field);
        }

        public static Action<TEntity, TProp> ToSetter<TEntity, TProp>(this PropertyInfo prop)
        {
            return _getSetter<TEntity, TProp>(prop);
        }

        public static Action<TEntity, TField> ToSetter<TEntity, TField>(this FieldInfo field)
        {
            return _getSetter<TEntity, TField>(field);
        }

        public static Func<TEntity, TValue> ToGetter<TEntity, TValue>(this MemberInfo member)
        {
            if (member is PropertyInfo prop)
            {
                return prop.ToGetter<TEntity, TValue>();
            }

            if (member is FieldInfo field)
            {
                return field.ToGetter<TEntity, TValue>();
            }

            throw new ArgumentException("Member must either be PropertyInfo or FieldInfo", nameof(member));
        }

        public static Action<TEntity, TValue> ToSetter<TEntity, TValue>(this MemberInfo member)
        {
            if (member is PropertyInfo prop)
            {
                return prop.ToSetter<TEntity, TValue>();
            }

            if (member is FieldInfo field)
            {
                return field.ToSetter<TEntity, TValue>();
            }

            throw new ArgumentException("Member must either be PropertyInfo or FieldInfo", nameof(member));
        }

    }
}
