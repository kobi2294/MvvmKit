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
            MemberExpression member = source.GetMemberExpression();

            PropertyInfo res = (member == null) ? null : member.Member as PropertyInfo;

            if (res == null)
            {
                throw new ArgumentException("The body must be a property expression");
            }

            return res;
        }

        public static PropertyInfo GetProperty<T1, T2>(this Expression<Func<T1, T2>> source)
        {
            MemberExpression body = source.GetMemberExpression();

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

        public static MemberExpression GetMemberExpression<T1, T2>(this Expression<Func<T1, T2>> expression)
        {
            MemberExpression memberExpression = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression)expression.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression.Body as MemberExpression;
            }

            if (memberExpression == null)
            {
                throw new ArgumentException("Not a member access", "expression");
            }

            return memberExpression;
        }

        public static MemberExpression GetMemberExpression<T>(this Expression<Func<T>> expression)
        {
            MemberExpression memberExpression = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression)expression.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression.Body as MemberExpression;
            }

            if (memberExpression == null)
            {
                throw new ArgumentException("Not a member access", "expression");
            }

            return memberExpression;
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
