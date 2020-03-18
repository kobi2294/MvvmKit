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
        public static void Run()
        {
            var container = new UnityContainer();
            container.RegisterType<IResolver, UnityResolver>(new ContainerControlledLifetimeManager());
            var vm = container.Resolve<ViewModel>();
            var model = ImmutableList.Create(
                new ItemModel("1", "First", "Odd"),
                new ItemModel("2", "Second", "Even"),
                new ItemModel("3", "Third", "Odd"),
                new ItemModel("4", "Fourth", "Even"),
                new ItemModel("5", "Fifth", "Odd"));

            Console.WriteLine(model.ToJson());

            var subj = new BehaviorSubject<ImmutableList<ItemModel>>(model);
            vm.Initialize(subj);

            Console.WriteLine("------------------------------------");

            model = model
                .RemoveAt(2)
                .Move(2, 3);

            Console.WriteLine(model.ToJson());

            subj.OnNext(model);

            Console.WriteLine("------------------------------------");

            model = model
                .SetItem(1, new ItemModel("6", "Sixth", "Even"))
                .Add(new ItemModel("7", "Seventh", "Odd"))
                .RemoveAll(m => m.Category == "Odd");

            Console.WriteLine(model.ToJson());
            subj.OnNext(model);

        }
    }
}
