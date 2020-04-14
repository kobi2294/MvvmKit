using MvvmKit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace ConsoleDemo.Samples.RxMvvm
{
    public static class Main
    {
        private static int _counter = 10;
        private static ViewModel _vm;
        private static ImmutableList<ItemModel> _model;
        private static UnityContainer _container;
        private static BehaviorSubject<ImmutableList<ItemModel>> _subj;

        private static void _do(ImmutableList<ItemModel> model)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("-----------------------------------------");
            Console.ForegroundColor = ConsoleColor.White;
            _model = model;
            var str = _model.ToJson();
            Console.WriteLine(str);
            Console.ForegroundColor = ConsoleColor.Yellow;
            _subj.OnNext(_model);            
        }

        public static void TestDisposableAsObservable()
        {
            Console.WriteLine(DateTime.Now);
            var disposable = new BaseDisposable();
            var observable = disposable.AsObservable<int>(() => 42).Timestamp();

            observable.Subscribe(i => Console.WriteLine("ob 1 Next " + i), () => Console.WriteLine("ob 1 Completed"));
            var sub = observable.Subscribe(i => Console.WriteLine("ob 2 Next " + i), () => Console.WriteLine("ob 2 Completed"));

            Task.Delay(1000).Wait();

            sub.Dispose();

            Task.Delay(1000).Wait();

            disposable.Dispose();

            Task.Delay(1000).Wait();

            observable.Subscribe(i => Console.WriteLine("ob 3 Next " + i), () => Console.WriteLine("ob 3 Completed"));

        }

        public static void TestCompletedByDisposable()
        {
            var disposable = new BaseDisposable();
            var obs = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .CompletedBy(disposable)
                .Timestamp();

            obs.Subscribe(i => Console.WriteLine("ob 1 Next " + i), () => Console.WriteLine("ob 1 Completed"));
            Task.Delay(2200).Wait();
            obs.Subscribe(i => Console.WriteLine("ob 2 Next " + i), () => Console.WriteLine("ob 2 Completed"));
            Task.Delay(2200).Wait();
            var sub = obs.Subscribe(i => Console.WriteLine("ob 3 Next " + i), () => Console.WriteLine("ob 3 Completed"));
            Task.Delay(2200).Wait();
            sub.Dispose();
            Task.Delay(2200).Wait();
            disposable.Dispose();

        }

        public static void TestObservableCollection()
        {
            var oc = new ObservableCollection<string>();
            var obs = MvvmRx.CollectionChanges(oc);
            obs.Subscribe(val =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Join<string>(", ", val.oldValue));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(string.Join<string>(", ", val.newValue));
            });

            oc.Add("A");
            oc.Add("B");
            oc.Add("C");
            oc.Add("D");
            oc.Move(1, 3);
            oc[2] = "E";
            oc.RemoveAt(2);
            oc.Clear();
        }

        public static void Run()
        {
            _container = new UnityContainer();
            _container.RegisterType<IResolver, UnityResolver>(new ContainerControlledLifetimeManager());
            _vm = _container.Resolve<ViewModel>();


            Console.WriteLine(_model.ToJson());

            _subj = new BehaviorSubject<ImmutableList<ItemModel>>(ImmutableList<ItemModel>.Empty);
            _vm.Initialize(_subj);

            _do(ImmutableList.Create(
                new ItemModel("1", "First", "Odd"),
                new ItemModel("2", "Second", "Even"),
                new ItemModel("3", "Third", "Odd"),
                new ItemModel("4", "Fourth", "Even"),
                new ItemModel("5", "Fifth", "Odd")));

            // 1, 2, 5, 4
            _do(_model
                .RemoveAt(2)
                .Move(2, 3));

            // 6, 4
            _do(_model
                 .SetItem(1, new ItemModel("6", "Sixth", "Even"))
                 .Add(new ItemModel("7", "Seventh", "Odd"))
                 .RemoveAll(m => m.Category == "Odd"));

            // 6*, 4
            _do(_model
                .SetItem(0, _model[0].With(x => x.DisplayName, "New Sixth").With(x => x.Category, "Edited"))
                .RemoveAt(1)
                .Insert(0, new ItemModel("4", "New Fourth", "Edited")));

            _vm.AddItem.Subscribe(x =>
            {
                _do(_model
                    .Add(new ItemModel((++_counter).ToString(), "Item " + _counter, "Category " + _counter)));
            },
            () => Console.WriteLine("Command Completed")
            );

            _vm.RemoveItem.Subscribe(id =>
            {
                _do(_model.RemoveAll(x => x.Uid == id));
            });


            _vm.AddItem.Execute(null);
            _vm.AddItem.Execute(null);
            _vm.RemoveItem.Execute("4");

            MvvmRx.PropertyChanges(_vm, x => x.Caption)
                .Subscribe(x => Console.WriteLine($"Property Caption changed from {x.oldValue} to {x.newValue}"),
                () => Console.WriteLine("Property observer completed"));

            // property changes observable shoud fire two evnets
            _vm.Caption = "A";
            _vm.Caption = "B";



            _vm.Dispose();
        }
    }
}



