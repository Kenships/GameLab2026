using System;
using System.Collections.Generic;
using System.Linq;

namespace _Project.Scripts.Targeting
{
    public static class SortingUtil
    {
        public static List<T> GetFirstN<T>(int n, List<T> list, Func<T, T, bool> compare)
        {
            if (n <= 0)
            {
                return new List<T>();
            }
            
            Dictionary<T, HashSet<T>> comparisonHistory = new();

            T winner = FindWinner(list.Count, list, compare, comparisonHistory);
            list.Clear();
            list.Add(winner);
            
            for (int i = 1; i < n; i++)
            {
                if (!comparisonHistory.TryGetValue(winner, out HashSet<T> value))
                {
                    list.Add(winner);
                    continue;
                }
                List<T> newList = value.ToList();
                winner = FindWinner(newList.Count, newList, compare, comparisonHistory);
                list.Add(winner);
            }

            return list;
        }
        
        private static T FindWinner<T>(int endIndex, List<T> list, Func<T, T, bool> compare, Dictionary<T, HashSet<T>> comparisonHistory)
        {
            if (endIndex == 0)
            {
                return default;
            }
            if (endIndex == 1)
            {
                return list[0];
            }
            
            for (int start = 0; start < endIndex / 2; start++)
            {
                int end = endIndex - 1 - start;

                int winnerIndex;
                if (comparisonHistory.ContainsKey(list[start]) && comparisonHistory[list[start]].Contains(list[end]))
                    winnerIndex = start;
                else if(comparisonHistory.ContainsKey(list[end]) && comparisonHistory[list[end]].Contains(list[start]))
                    winnerIndex = end;
                else
                    winnerIndex = compare(list[start], list[end]) ? start : end;
                
                int loserIndex = winnerIndex == start ? end : start;
                if (!comparisonHistory.TryAdd(list[winnerIndex], new HashSet<T> { list[loserIndex] }))
                {
                    comparisonHistory[list[winnerIndex]].Add(list[loserIndex]);
                }

                if (winnerIndex > loserIndex)
                {
                    Swap(winnerIndex, loserIndex, list);
                }
            }
            
            return FindWinner((endIndex + 1)/2, list, compare, comparisonHistory);
        }

        private static void Swap<T>(int i, int j, List<T> list)
        {
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
