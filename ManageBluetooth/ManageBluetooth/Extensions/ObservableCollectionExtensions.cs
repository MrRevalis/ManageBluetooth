using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ManageBluetooth.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> value, IEnumerable<T> items)
        {
            if (items == null
                || !items.Any())
            {
                return;
            }

            foreach (var item in items)
            {
                value.Add(item);
            }
        }

        public static void Sort<TSource, TKey>(this ObservableCollection<TSource> source, Func<TSource, TKey> keySelector)
        {
            List<TSource> sortedList = source.OrderBy(keySelector).ToList();
            source.Clear();

            foreach (var sortedItem in sortedList)
            {
                source.Add(sortedItem);
            }
        }

        public static void SortByDescending<TSource, TKey>(this ObservableCollection<TSource> source, Func<TSource, TKey> keySelector)
        {
            List<TSource> sortedList = source.OrderByDescending(keySelector).ToList();
            source.Clear();

            foreach (var sortedItem in sortedList)
            {
                source.Add(sortedItem);
            }
        }

        public static int FindIndex<T>(this ObservableCollection<T> source, Predicate<T> match)
        {
            for (int i = 0; i < source.Count; i++)
            {
                if (match(source[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public static void Replace<T>(this ObservableCollection<T> source, Predicate<T> condition, T newValue)
        {
            var index = source.FindIndex(condition);

            if (index != -1)
            {
                source.Insert(index, newValue);
            }
        }

        public static void Replace<T>(this ObservableCollection<T> source, int index, T newValue)
        {
            if (index != -1)
            {
                source.Insert(index, newValue);
            }
        }

    }
}
