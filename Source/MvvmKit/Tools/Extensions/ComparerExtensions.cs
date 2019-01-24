using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class ComparerExtensions
    {
        public static IEqualityComparer<T> ToEqualityComparer<T>(this Func<T, T, bool> func)
        {
            return new FuncEqualityComparer<T>(func);
        }

        public static IComparer<T> ToComparer<T>(this Func<T, T, int> compareFunction)
        {
            return new FuncComparer<T>(compareFunction);
        }

        public static IComparer<T> ToComparer<T>(this Comparison<T> compareFunction)
        {
            return new ComparisonComparer<T>(compareFunction);
        }

        public static IComparer<string> ToComparer<T>(this CompareInfo compareInfo)
        {
            return new FuncComparer<string>(compareInfo.Compare);
        }

        #region Private

        private class FuncComparer<T> : IComparer<T>
        {
            public FuncComparer(Func<T, T, int> func)
            {
                m_func = func;
            }

            public int Compare(T x, T y)
            {
                return m_func(x, y);
            }

            private readonly Func<T, T, int> m_func;
        }

        private class ComparisonComparer<T> : IComparer<T>
        {
            public ComparisonComparer(Comparison<T> func)
            {
                m_func = func;
            }

            public int Compare(T x, T y)
            {
                return m_func(x, y);
            }

            private readonly Comparison<T> m_func;
        }

        private class FuncEqualityComparer<T> : IEqualityComparer<T>
        {
            public FuncEqualityComparer(Func<T, T, bool> func)
            {
                m_func = func;
            }
            public bool Equals(T x, T y)
            {
                return m_func(x, y);
            }

            public int GetHashCode(T obj)
            {
                return 0; // this is on purpose. Should only use function...not short-cut by hashcode compare
            }

            private readonly Func<T, T, bool> m_func;
        }

        #endregion
    }
}
