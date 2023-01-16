using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace ManageBluetooth.Custom.Collection
{
    public class FilteredObservableCollection<T> : ObservableCollection<T>
    {
        private Predicate<T> Filter;

        public FilteredObservableCollection(ObservableCollection<T> source, Predicate<T> filter)
            : base(source.Where(item => filter(item)))
        {
            source.CollectionChanged += OnCollectionChanged;
            Filter = filter;
        }

        private void Fill(ObservableCollection<T> source)
        {
            Clear();

            foreach (T item in source)
            {
                if (Filter(item))
                {
                    Add(item);
                }
            }
        }

        private int this[T item]
        {
            get
            {
                int foundIndex = -1;
                for (int index = 0; index < Count; index++)
                {
                    if (this[index].Equals(item))
                    {
                        foundIndex = index;
                        break;
                    }
                }
                return foundIndex;
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ObservableCollection<T> source = (ObservableCollection<T>)sender;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (T item in e.NewItems)
                    {
                        if (Filter(item))
                        {
                            Add(item);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    Fill(source);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (T item in e.OldItems)
                    {
                        if (Filter(item))
                        {
                            Remove(item);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    for (int index = 0; index < e.OldItems.Count; index++)
                    {
                        T item = (T)e.OldItems[index];
                        if (Filter(item))
                        {
                            int foundIndex = this[item];

                            if (foundIndex == -1)
                            {
                                continue;
                            }

                            T newItem = (T)e.NewItems[index];

                            if (Filter(newItem))
                            {
                                this[foundIndex] = newItem;
                            }
                            else
                            {
                                RemoveAt(foundIndex);
                            }
                        }
                        else
                        {
                            Fill(source);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Fill(source);
                    break;
            }
        }
    }
}