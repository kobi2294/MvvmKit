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
    public static class LambdaExpressionFactory
    {
        public static DelegateType CompileTo<DelegateType>(this MethodInfo mi, params object[] constantArgs)
            where DelegateType: Delegate
        {
            var signature = Signature.Of<DelegateType>();
            var parameters = _createParameterExpression(signature.ParameterTypes);

            var expectedArgumentTypes = _getMethodInfoExpectedArgumentTypes(mi);
            var callArguments = _createArgumentExpressions(constantArgs, parameters, expectedArgumentTypes);

            Expression call = null;
            if (mi.IsStatic)
            {
                call = Expression.Call(mi, callArguments);
            } else
            {
                call = Expression.Call(callArguments.First(), mi, callArguments.Skip(1));
            }

            call = call.EnsureConvert(signature.ReturnType);
            return Expression
                .Lambda<DelegateType>(call, parameters)
                .Compile();
        }

        public static DelegateType CompileTo<DelegateType>(this ConstructorInfo ci, params object[] constantArgs)
            where DelegateType : Delegate
        {
            var signature = Signature.Of<DelegateType>();
            var parameters = _createParameterExpression(signature.ParameterTypes);

            var expectedArgumentTypes = _getMethodInfoExpectedArgumentTypes(ci);
            var callArguments = _createArgumentExpressions(constantArgs, parameters, expectedArgumentTypes);

            Expression call = Expression
                .New(ci, callArguments)
                .EnsureConvert(signature.ReturnType);

            return Expression
                .Lambda<DelegateType>(call, parameters)
                .Compile();
        }

        public static Func<object[], T> CompileToArrayFunc<T>(this MethodInfo mi, params object[] constantArgs)
        {
            var signature = Signature.Of<Func<object[], T>>();
            var parameter = _createParameterExpression(signature.ParameterTypes).Single();

            var expectedArgumentTypes = mi.GetParameters()
                .Select(parm => parm.ParameterType);

            var callArguments = _createArgumentExpressionsFromArray(constantArgs, parameter, expectedArgumentTypes);

            Expression call = null;
            if (mi.IsStatic)
            {
                call = Expression.Call(mi, callArguments);
            }
            else
            {
                call = Expression.Call(callArguments.First(), mi, callArguments.Skip(1));
            }

            call = call.EnsureConvert(signature.ReturnType);
            return Expression
                .Lambda<Func<object[], T>>(call, parameter)
                .Compile();
        }

        public static Func<object[], T> CompileToArrayFunc<T>(this ConstructorInfo ci, params object[] constantArgs)
        {
            var signature = Signature.Of<Func<object[], T>>();
            var parameter = _createParameterExpression(signature.ParameterTypes).Single();

            var expectedArgumentTypes = ci.GetParameters()
                .Select(parm => parm.ParameterType);

            var callArguments = _createArgumentExpressionsFromArray(constantArgs, parameter, expectedArgumentTypes);

            Expression call = Expression
                .New(ci, callArguments)
                .EnsureConvert(signature.ReturnType);

            return Expression
                .Lambda<Func<object[], T>>(call, parameter)
                .Compile();
        }


        private static IEnumerable<ParameterExpression> _createParameterExpression(IEnumerable<Type> types)
        {
            return types
                .Select(type => Expression.Parameter(type))
                .ToList();
        }

        private static IEnumerable<Expression> _createArgumentExpressions(object[] providedConstants, 
            IEnumerable<Expression> providedExpressions, 
            IEnumerable<Type> expectedArguments)
        {
            var readyArgs = providedConstants
                .Select(constant => Expression.Constant(constant))
                .Concat(providedExpressions);

            return expectedArguments.ZipThen(readyArgs,
                (type, arg) => arg.EnsureConvert(type),
                type => Expression.Constant(type.DefaultValue(), type))
                .ToList();
        }

        private static IEnumerable<Expression> _createArgumentExpressionsFromArray(object[] providedConstants, 
            Expression arrayExpression, 
            IEnumerable<Type> expectedArguments)
        {
            var readyArgs = providedConstants
                .Select(constant => Expression.Constant(constant));

            return expectedArguments.ZipThen(readyArgs,
                (type, arg) => arg.EnsureConvert(type),
                (type, index) => Expression
                                    .ArrayIndex(arrayExpression, Expression.Constant(index))
                                    .EnsureConvert(type))
                .ToList();
        }

        private static IEnumerable<Type> _getMethodInfoExpectedArgumentTypes(MethodBase mi)
        {
            var expectedArgumentTypes = mi.GetParameters()
                .Select(parm => parm.ParameterType);

            if ((!mi.IsStatic) && (mi is MethodInfo)) expectedArgumentTypes = expectedArgumentTypes.StartWith(mi.DeclaringType);
            return expectedArgumentTypes;
        }
    }
}
