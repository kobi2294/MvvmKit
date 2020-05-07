using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit
{
    public static class ObservableCollectionExtensions
    {
        public static string AsString(this NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    return $"Add [{string.Join(", ", args.NewItems.Cast<object>())}]";
                case NotifyCollectionChangedAction.Remove:
                    return $"Remove [{string.Join(", ", args.OldItems.Cast<object>())}]";
                case NotifyCollectionChangedAction.Replace:
                    return $"Replace [{string.Join(", ", args.OldItems.Cast<object>())}] -> [{string.Join(", ", args.NewItems.Cast<object>())}]";
                case NotifyCollectionChangedAction.Move:
                    return $"Move {args.OldStartingIndex} -> {args.NewStartingIndex}";
                case NotifyCollectionChangedAction.Reset:
                    return $"Reset";
                default:
                    return "";
            }
        }

        public static ObservableCollection<T> AddRange<T>(this ObservableCollection<T> source, IEnumerable<T> items)
        {
            foreach (var item in items.ToList())
            {
                source.Add(item);
            }
            return source;
        }
    }
}
