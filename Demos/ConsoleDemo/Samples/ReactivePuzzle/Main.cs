using Castle.Core;
using MvvmKit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.ReactivePuzzle
{
    public static class Main
    {
        private static object _mutex = new object();

        public static void Run()
        {
            var dashboards = _createDashboardsObservable();
            var displays = _createDisplaysObservable();

            dashboards
                .Select(dict => dict.ToConsoleString())
                .SubscribeToConsole("dashboards", ConsoleColor.Cyan);

            displays
                .Select(list => list.ToConsoleString())
                .SubscribeToConsole("displays", ConsoleColor.Red);

            // 6. subscribe and fire

            dashboards
            // 1. combine displays and dashboards to [(displayId, dashboard?)]
                .CombineLatest(displays, (dashs, disps) => _combine(dashs, disps))
            // 2. select many to Observable<(displayId, dashboard?)>
                .SelectMany(many => many)
            // 3. group by displayId
                .GroupBy(tpl => tpl.displayId)
            // 4. each observable: observable.distinctuntilchanged.where not null
                .Select(go => go
                                .DistinctUntilChanged()
                                .Where(tpl => tpl.value != null))
            // 5. merge
                .Merge()
                .SubscribeToConsole("select many", ConsoleColor.Green);

        }

        public static ImmutableList<(int displayId, string value)> _combine(ImmutableDictionary<int, string> dashboards, ImmutableList<(int displayId, bool isSync, int dashboardId)> displays)
        {
            return displays
                .Select(tpl => (displayId: tpl.displayId, value: (tpl.isSync && dashboards.ContainsKey(tpl.dashboardId))
                                                                    ? dashboards[tpl.dashboardId]
                                                                    : (string)null))
                .ToImmutableList();
        }

        public static IDisposable SubscribeToConsole<T>(this IObservable<T> source, string prefix, ConsoleColor color)
        {
            return source.Subscribe(
                onNext: val => PrintWithColor($"{prefix} Next: {val}", color),
                onCompleted: () => PrintWithColor($"{prefix} Completed", color),
                onError: err => PrintWithColor($"{prefix} Error: {err.Message}", color)
                );
        }

        public static void PrintWithColor(string txt, ConsoleColor color)
        {
            lock (_mutex)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(txt);
            }
        }


        // like dashboards
        private static IObservable<ImmutableDictionary<int, string>> _createDashboardsObservable()
        {
            return Observable.Generate(
                initialState: _createRandomDictionary(),
                condition: dict => true,
                iterate: dict => _mutate(dict),
                resultSelector: dict => dict,
                timeSelector: dict => TimeSpan.FromSeconds(2))
                .Publish()
                .RefCount();
        }

        private static IObservable<ImmutableList<(int displayId, bool isSync, int dashboardId)>> _createDisplaysObservable()
        {
            return Observable.Generate(
                initialState: _createInitialDisplays(),
                condition: list => true,
                iterate: list => _mutate(list),
                resultSelector: list => list,
                timeSelector: list => TimeSpan.FromSeconds(5))
                .Publish()
                .RefCount();
        }

        private static ImmutableList<(int displayId, bool isSync, int dashboardId)> _createInitialDisplays()
        {
            return Enumerable
                .Range(20, 5)
                .Select(i => (displayId: i, isSync: Randoms.Default.Toss(0.8), dashboardId: Randoms.Default.Next(0, 10)))
                .ToImmutableList();                
        }

        private static ImmutableList<(int displayId, bool isSync, int dashboardId)> _mutate(ImmutableList<(int displayId, bool isSync, int dashboardId)> source)
        {
            var toss = Randoms.Default.Toss();
            var index = Randoms.Default.Next(0, source.Count);
            var item = source[index];

            if (toss)
            {
                // change one display isSync
                return source.SetItem(index, (item.displayId, !item.isSync, item.dashboardId));
            }
            else
            {
                // change one display dashboard id
                return source.SetItem(index, (item.displayId, item.isSync, Randoms.Default.Next(0, 10)));
            }
        }

        private static ImmutableDictionary<int, string> _mutate(ImmutableDictionary<int, string> source)
        {
            var randomKey = Randoms.Default.OneOf(source.Keys);
            var randomValue = Randoms.Default.NextWords(1);

            return source.SetItem(randomKey, randomValue);
        }

        private static ImmutableDictionary<int, string> _createRandomDictionary()
        {
            return Enumerable
                .Range(1, 10)
                .Select(i => (key: i, value: Randoms.Default.NextWords(1)))
                .ToImmutableDictionary(pair => pair.key, pair => pair.value);

        }

        public static string ToConsoleString(this ImmutableDictionary<int, string> dict)
        {
            return dict
                .Select(pair => $"[{pair.Key}:{pair.Value}]".PadRight(17))
                .Join("");
        }

        public static string ToConsoleString(this ImmutableList<(int displayId, bool isSync, int dashboardId)> source) 
        {
            return source
                .Select(tpl => $"[{tpl.displayId}, {tpl.isSync}, {tpl.dashboardId}]".PadRight(17))
                .Join("");
        }

        public static string ToConsoleString(this ImmutableList<(int displayId, string value)> source)
        {
            return source
                .Select(tpl => $"[{tpl.displayId}, {tpl.value}]".PadRight(17))
                .Join("");
        }

    }
}
