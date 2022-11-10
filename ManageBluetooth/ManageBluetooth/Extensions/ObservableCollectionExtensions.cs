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
    }
}
