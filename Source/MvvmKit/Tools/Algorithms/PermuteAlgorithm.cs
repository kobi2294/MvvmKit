using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class PermuteAlgorithm
    {
        /// <summary>
        /// Returns a list of moves to be made, to permute a sorted range (0..target.length-1) into target
        /// </summary>
        /// <param name="target">A permutation containing sequential numbers starting from 0, by desired order</param>
        /// <returns>List of moves, that if done in their order, transfer the ordered range 0, 1, 2... into target</returns>
        public static IEnumerable<(int from, int to)> PermuteIndices(this IEnumerable<int> target)
        {
            // Example: 
            // 3 1 9 5 4 2 0 6 8 7
            // Step 1: inverse indices - for each number 0 .. 9 write its final index
            // 6 1 5 0 4 3 7 9 8 2 (meaning, 0 will be in index 6, 1 in 1, 5 in 3 and so on)
            // Step 2: find LIS
            //   1       3 7   8
            // Step 3: The rest are the movables: 
            // 6   5 0 4     9   2
            // Step 4: move them from smallest to largest. Each time, item n should move so that it is after n-1. 0 should be moved to first
            // remember each instruction you perform
            // 6 1 5 0 4 3 7 9 8 2
            // move 0 to start      (from: 3, to: 0)
            // 0 6 1 5 4 3 7 9 8 2
            // move 2 after 1       (from: 9, to: 3)
            // 0 6 1 2 5 4 3 7 9 8
            // move 4 after 3       (from: 5, to: 6)
            // 0 6 1 2 5 3 4 7 9 8
            // move 5 after 4       (from: 4, to: 6)
            // 0 6 1 2 3 4 5 7 9 8
            // move 6 after 5       (from: 1, to: 6)
            // 0 1 2 3 4 5 6 7 9 8
            // move 9 after 8       (from: 8, to: 9)
            // 0 1 2 3 4 5 6 7 8 9
            // Done! return the set of instructions

            // now if you perform the exact set of instructions on 0..1..2..3... you will get the target
            // 0 1 2 3 4 5 6 7 8 9  -- (from: 3, to: 0) --> 
            // 3 0 1 2 4 5 6 7 8 9  -- (from: 9, to: 3) --> 
            // 3 0 1 9 2 4 5 6 7 8  -- (from: 5, to: 6) --> 
            // 3 0 1 9 2 5 4 6 7 8  -- (from: 4, to: 6) --> 
            // 3 0 1 9 5 4 2 6 7 8  -- (from: 1, to: 6) --> 
            // 3 1 9 5 4 2 0 6 7 8  -- (from: 8, to: 9) -->
            // 3 1 9 5 4 2 0 6 8 7 

            // map holds for each number, it's index in target
            var map = target
                .Enumerated()
                .ToDictionary(pair => pair.item, pair => pair.index);
            var inverted = Enumerable.Range(0, map.Count)
                                .Select(i => map[i])
                                .ToAvlList();
            var lis = inverted.Lis();
            var movables = inverted.Except(lis)
                                   .OrderBy(i => i)
                                   .ToList();

            var curList = inverted;

            foreach (var item in movables)
            {
                var sourceIndex = curList.IndexOf(item);
                var targetIndex = 0;

                // if item is 0 - the target will always be 0, otherwise, we need to find the anchor
                if (item > 0)
                {
                    var anchor = item - 1;
                    var anchorIndex = curList.IndexOf(anchor);

                    if (sourceIndex > anchorIndex)
                        targetIndex = anchorIndex + 1;
                    else
                        targetIndex = anchorIndex;
                }

                yield return (from: sourceIndex, to: targetIndex);
                curList.RemoveAt(sourceIndex);
                curList.InsertAt(targetIndex, item);
            }
        }


        public static IEnumerable<(int from, int to)> Permute<T>(this IEnumerable<T> source, IEnumerable<T> target)
        {
            var targetIndices = target.IndicesIn(source);
            return targetIndices.PermuteIndices();
        }
    }
}
