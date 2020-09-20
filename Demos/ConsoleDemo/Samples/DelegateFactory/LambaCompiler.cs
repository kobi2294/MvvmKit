using MvvmKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.DelegateFactory
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
