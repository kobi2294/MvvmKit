using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class LisAlgorithm
    {

        // Binary search  
        private static int _getCeilIndex(int[] data, int[] tailIndices, int left,
                                int right, int key)
        {

            while (right > left + 1)
            {
                int middle = left + (right - left) / 2;

                if (data[tailIndices[middle]] >= key)
                    right = middle;
                else
                    left = middle;
            }

            return right;
        }

        private static T[] _initArray<T>(int length, T item = default(T))
        {
            var res = new T[length];
            for (int i = 0; i < length; i++)
            {
                res[i] = item;
            }
            return res;
        }


        private static int[] _collectResults(int len, int[] tailIndices, int[] data, int[] prevIndices)
        {
            var res = new int[len];
            int i = tailIndices[len - 1];
            int j = len - 1;
            while (i >= 0)
            {
                res[j--] = data[i];
                i = prevIndices[i];
            }
            return res;
        }

        public static int[] Calculate(int[] data)
        {
            int length = data.Length;
            if (length == 0) return new int[0];

            int[] tailIndices = _initArray(length, 0);
            int[] prevIndices = _initArray(length, -1);

            // it will always point to empty 
            // location 
            int len = 1;

            for (int i = 1; i < length; i++)
            {
                if (data[i] < data[tailIndices[0]])

                    // new smallest value 
                    tailIndices[0] = i;

                else if (data[i] > data[tailIndices[len - 1]])
                {

                    // arr[i] wants to extend 
                    // largest subsequence 
                    prevIndices[i] = tailIndices[len - 1];
                    tailIndices[len++] = i;
                }
                else
                {

                    // arr[i] wants to be a potential 
                    // condidate of future subsequence 
                    // It will replace ceil value in 
                    // tailIndices 
                    int pos = _getCeilIndex(data, tailIndices, -1, len - 1, data[i]);

                    prevIndices[i] = tailIndices[pos - 1];
                    tailIndices[pos] = i;
                }
            }

            // collect results

            return _collectResults(len, tailIndices, data, prevIndices);
        }

        public static int[] Lis(this IEnumerable<int> data)
        {
            return Calculate(data.ToArray());
        }

        public static T[] LisBy<T>(this IEnumerable<T> target, IEnumerable<T> original)
        {
            var originalList = original.ToList();
            var indices = target.IndicesIn(original);
            var lis = indices.Lis();
            return lis.Select(i => originalList[i]).ToArray();
        }
    }
}
