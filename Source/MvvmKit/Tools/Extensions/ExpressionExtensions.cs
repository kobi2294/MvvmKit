using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class ExpressionExtensions
    {
        public static PropertyInfo GetProperty<T>(this Expression<Func<T>> source)
        {
            MemberExpression body = source.Body as MemberExpression;

            PropertyInfo res = (body == null) ? null : body.Member as PropertyInfo;

            if (res == null)
            {
                throw new ArgumentException("The body must be a property expression");
            }

            return res;
        }

        public static PropertyInfo GetProperty<T1, T2>(this Expression<Func<T1, T2>> source)
        {
            MemberExpression body = source.Body as MemberExpression;

            PropertyInfo res = (body == null) ? null : body.Member as PropertyInfo;

            if (res == null)
            {
                throw new ArgumentException("The body must be a property expression");
            }

            return res;
        }

        public static PropertyInfo GetProperty(this LambdaExpression source)
        {
            MemberExpression body = source.Body as MemberExpression;

            PropertyInfo res = (body == null) ? null : body.Member as PropertyInfo;

            if (res == null)
            {
                throw new ArgumentException("The body must be a property expression");
            }

            return res;
        }


        public static String GetName<T>(this Expression<Func<T>> source)
        {
            return source.GetProperty().Name;
        }

        public static String GetName<T1, T2>(this Expression<Func<T1, T2>> source)
        {
            return source.GetProperty().Name;
        }

        public static String GetName(this LambdaExpression source)
        {
            return source.GetProperty().Name;
        }

        public static bool IsNameOf<T>(this string name, Expression<Func<T>> source)
        {
            return source.GetName().Equals(name);
        }

        public static bool IsNameOf<T1, T2>(this string name, Expression<Func<T1, T2>> source)
        {
            return source.GetName().Equals(name);
        }

        public static bool IsNameOf(this string name, LambdaExpression source)
        {
            return source.GetName().Equals(name);
        }
    }
}
