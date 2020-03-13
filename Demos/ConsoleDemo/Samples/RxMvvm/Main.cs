using MvvmKit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo.Samples.RxMvvm
{
    public static class Main
    {
        public static void Run()
        {
            var vm = new ViewModel();
            var model = ImmutableList.Create(
                new ItemModel("1", "First", "Odd"),
                new ItemModel("2", "Second", "Even"),
                new ItemModel("3", "Third", "Odd"),
                new ItemModel("4", "Fourth", "Even"),
                new ItemModel("5", "Fifth", "Odd"));

            var subj = new BehaviorSubject<ImmutableList<ItemModel>>(model);
            vm.Initialize(subj);

            model = model
                .RemoveAt(2)
                .Move(2, 4);
            subj.OnNext(model);


            

        }
    }
}
