using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.LinqProvider
{
    public static class MyTypeExtensions
    {
        public static MyType<T> Where<T>(this MyType<T> source, Expression<Func<T, bool>> predicate)
        {
            source.Add("Where " + predicate.ToString());
            return source;
        }

        public static MyType<T> OrderBy<T, K>(this MyType<T> source, Expression<Func<T, K>> key)
        {
            source.Add("Order By " + key.ToString());
            return source;
        }

        public static MyType<T> OrderByDescending<T, K>(this MyType<T> source, Expression<Func<T, K>> key)
        {
            source.Add("Order By desc " + key.ToString());
            return source;
        }

        public static MyType<T> ThenBy<T, K>(this MyType<T> source, Expression<Func<T, K>> key)
        {
            source.Add("Then By " + key.ToString());
            return source;
        }

        public static MyType<T> ThenByDescending<T, K>(this MyType<T> source, Expression<Func<T, K>> key)
        {
            source.Add("Then By desc " + key.ToString());
            return source;
        }

        public static MyType<K> Select<T, K>(this MyType<T> source, Expression<Func<T, K>> projection)
        {
            source.Add("Select " + projection.ToString());
            return source.To<K>();
        }

        public static MyType<TResult> SelectMany<TSource, TCollection, TResult>(this MyType<TSource> source, 
            Expression<Func<TSource, MyType<TCollection>>> collectionSelector, 
            Expression<Func<TSource, TCollection, TResult>> selector
            )
        {
            source.Add("Select many " + collectionSelector.ToString() + ", " + selector.ToString());
            return source.To<TResult>();
        }

    }
}
