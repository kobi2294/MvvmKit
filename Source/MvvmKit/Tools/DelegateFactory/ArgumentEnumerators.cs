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
    public delegate IEnumerable<Expression> ArgumentEnumerator(List<ParameterExpression> expressions);

    public static class ArgumentEnumerators
    {
        public static ArgumentEnumerator Default { get; } = prms => prms.Cast<Expression>();

        public static ArgumentEnumerator ForArray { get; } = prms => 
            EnumerableExtensions
            .InfiniteRange(0)
            .Select(i => Expression.ArrayIndex(prms[0], Expression.Constant(i)));

        public static ArgumentEnumerator ForDetails(IEnumerable<Func<List<ParameterExpression>, Expression>> details)
        {
            return prms => details.Select(func => func(prms));
        }

        public static ArgumentEnumerator ForCopyCtor(MethodBase mb, params PropertyInfo[] changedProperties)
        {
            return ForCopyCtor(mb, changedProperties.Cast<PropertyInfo>());
        }

        public static ArgumentEnumerator ForCopyCtor(MethodBase mb, IEnumerable<PropertyInfo> changedProperties) 
        {
            var objectType = mb.DeclaringType;
            var arguments = mb.GetParameters().Select(pi => pi.Name.ToLower());

            var props = objectType.GetProperties().ToDictionary(prop => prop.Name.ToLower());
            var changedPropsIndex = changedProperties
                .Select((prop, index) => (prop, index))
                .ToDictionary(pair => pair.prop.Name.ToLower(), pair => pair.index);

            return prms =>
            {
                var source = prms[0].EnsureConvert(objectType);
                return arguments
                .Select(name => changedPropsIndex.ContainsKey(name)
                            ? (Expression)prms[changedPropsIndex[name] + 1]
                            : Expression.Property(source, props[name]));
            };
        }

        public static ArgumentEnumerator ForFunc<T>(MethodBase mb, Func<T, Type, object> func)
        {
            var argumentTypes = mb.GetParameters().Select(pi => pi.ParameterType);

            return prms =>
            {
                var source = prms[0].EnsureConvert(typeof(T));
                return argumentTypes.Select(type =>
                        Expression.Invoke(Expression.Constant(func), source, Expression.Constant(type, typeof(Type))));                                  
            };
        }


    }
}
