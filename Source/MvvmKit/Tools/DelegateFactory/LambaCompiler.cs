using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class LambaCompiler
    {
        public static DelegateType CompileTo<DelegateType>(this MethodBase mb)
        {
            return _compileTo<DelegateType>(mb);
        }

        public static DelegateType CompileTo<DelegateType>(this MethodBase mb, params Func<List<ParameterExpression>, Expression>[] argumentEvaluators)
        {
            return _compileTo<DelegateType>(mb, argumentEnumerator: ArgumentEnumerators.ForDetails(argumentEvaluators));
        }

        public static DelegateType CompileTo<DelegateType>(this MethodBase mb, params object[] constants)
        {
            return _compileTo<DelegateType>(mb, constants);
        }

        public static DelegateType CompileTo<DelegateType>(this MethodBase mb, IEnumerable<object> constants, ArgumentEnumerator argumentEnumerator)
        {
            return _compileTo<DelegateType>(mb, 
                constants??Enumerable.Empty<object>(), 
                argumentEnumerator??ArgumentEnumerators.Default);
        }

        public static Func<object[], T> CompileToArrayFunc<T>(this MethodBase mb, params object[] constantArgs)
        {
            return _compileTo<Func<object[], T>>(mb, constantArgs, ArgumentEnumerators.ForArray);
        }

        public static Func<T, TProp, T> CompileCopyConstructor<T, TProp>(this MethodBase mb, PropertyInfo prop)
        {
            return _compileTo<Func<T, TProp, T>>(mb, argumentEnumerator: ArgumentEnumerators.ForCopyCtor(mb, prop));
        }

        public static Func<TEntity, TProp> CompileGetter<TEntity, TProp>(this PropertyInfo pi)
        {
            return _compileTo<Func<TEntity, TProp>>(pi.GetMethod, argumentEnumerator: ArgumentEnumerators.Default);            
        }

        public static Action<TEntity, TProp> CompileSetter<TEntity, TProp>(this PropertyInfo pi)
        {
            return _compileTo<Action<TEntity, TProp>>(pi.SetMethod, argumentEnumerator: ArgumentEnumerators.Default);
        }

        public static Func<TEntity, TField> CompileGetter<TEntity, TField>(this FieldInfo fi)
        {
            return _compileFieldGetter<TEntity, TField>(fi);
        }

        public static Action<TEntity, TField> CompileSetter<TEntity, TField>(this FieldInfo fi)
        {
            return _compileFieldSetter<TEntity, TField>(fi);
        }

        public static Func<TEntity, TValue> CompileGetter<TEntity, TValue>(this MemberInfo mi)
        {
            if (mi is PropertyInfo pi) return pi.CompileGetter<TEntity, TValue>();
            if (mi is FieldInfo fi) return fi.CompileGetter<TEntity, TValue>();

            throw new ArgumentException("Expected PropertyInfo or FieldInfo", nameof(mi));
        }

        public static Action<TEntity, TValue> CompileSetter<TEntity, TValue>(this MemberInfo mi)
        {
            if (mi is PropertyInfo pi) return pi.CompileSetter<TEntity, TValue>();
            if (mi is FieldInfo fi) return fi.CompileSetter<TEntity, TValue>();

            throw new ArgumentException("Expected PropertyInfo or FieldInfo", nameof(mi));
        }


        private static DelegateType _compileTo<DelegateType>(MethodBase mb, 
            IEnumerable<object> constants = null, 
            ArgumentEnumerator argumentEnumerator = null)
        {
            constants = constants ?? Enumerable.Empty<object>();
            argumentEnumerator = argumentEnumerator ?? ArgumentEnumerators.Default;

            var signature = Signature.Of<DelegateType>();
            var parameters = _createParameterExpressions(signature.ParameterTypes);

            var expectedArgumentTypes = _getMethodExpectedArgumentTypes(mb);
            var callArguments = _createArgumentExpressions(constants, argumentEnumerator, parameters, expectedArgumentTypes);

            Expression call = null;
            if (mb is MethodInfo mi)
            {
                if (mi.IsStatic)
                {
                    call = Expression.Call(mi, callArguments);
                }
                else
                {
                    call = Expression.Call(callArguments.First(), mi, callArguments.Skip(1));
                }
            }
            else if (mb is ConstructorInfo ci)
            {
                call = Expression.New(ci, callArguments);
            }

            call = call.EnsureConvert(signature.ReturnType);
            return Expression
                .Lambda<DelegateType>(call, parameters)
                .Compile();
        }

        private static Func<TEntity, TValue> _compileFieldGetter<TEntity, TValue>(FieldInfo fi)
        {
            var instancePrm = Expression.Parameter(typeof(TEntity));

            var instanceExp = instancePrm.EnsureConvert(fi.DeclaringType);
            var fieldAccessExp = Expression.Field(instanceExp, fi).EnsureConvert(typeof(TValue));

            return Expression.Lambda<Func<TEntity, TValue>>(fieldAccessExp, instancePrm)
                .Compile();
        }

        private static Action<TEntity, TValue> _compileFieldSetter<TEntity, TValue>(FieldInfo fi)
        {
            var instancePrm = Expression.Parameter(typeof(TEntity));
            var valuePrm = Expression.Parameter(typeof(TValue));

            var instanceExp = instancePrm.EnsureConvert(fi.DeclaringType);
            var valueExp = valuePrm.EnsureConvert(fi.FieldType);

            var fieldAssignExp = Expression.Assign(Expression.Field(instanceExp, fi), valueExp);
            return Expression.Lambda<Action<TEntity, TValue>>(fieldAssignExp, instancePrm, valuePrm)
                .Compile();
        }

        private static List<ParameterExpression> _createParameterExpressions(IEnumerable<Type> types)
        {
            return types
                .Select(type => Expression.Parameter(type))
                .ToList();
        }

        private static IEnumerable<Expression> _createArgumentExpressions(IEnumerable<object> constantArgument,
            ArgumentEnumerator argumentEnumerator,
            List<ParameterExpression> parameters,
            IEnumerable<Type> expectedArguments)
        {
            var calculatedExpressions = argumentEnumerator(parameters);

            var readyArgs = constantArgument
                .Select(constant => Expression.Constant(constant))
                .Concat(calculatedExpressions);

            return expectedArguments.ZipThen(readyArgs,
                (type, arg) => arg.EnsureConvert(type),
                type => Expression.Constant(type.DefaultValue(), type))
                .ToList();
        }

        private static IEnumerable<Type> _getMethodExpectedArgumentTypes(MethodBase mi)
        {
            var expectedArgumentTypes = mi.GetParameters()
                .Select(parm => parm.ParameterType);

            if ((!mi.IsStatic) && (mi is MethodInfo)) expectedArgumentTypes = expectedArgumentTypes.StartWith(mi.DeclaringType);
            return expectedArgumentTypes;
        }
    }
}
