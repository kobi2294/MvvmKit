using MvvmKit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace ConsoleDemo.Samples.RxMvvm
{
    public static class Main
    {
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

        }
    }
}



