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
    }
}
