using MvvmKit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace SelectiveResourcesDemo.SelectManyDemo
{
    public class SelectManyDemoVm: BindableBase
    {
        private BehaviorSubject<ImmutableList<ItemModel>> _subject;

        #region Properties

        private ObservableCollection<ItemVm> _Items;
        public ObservableCollection<ItemVm> Items { get { return _Items; } set { SetProperty(ref _Items, value); } }


        private ObservableCollection<ItemVm> _SelectedIds;
        public ObservableCollection<ItemVm> SelectedIds { get { return _SelectedIds; } set { SetProperty(ref _SelectedIds, value); } }

        #endregion

        public SelectManyDemoVm()
        {
            Items = new ObservableCollection<ItemVm>().AllDisposedBy(this);
            SelectedIds = new ObservableCollection<ItemVm>();
            Initialize();
        }

        public async void Initialize()
        {
            var model = Enumerable.Empty<ItemModel>()
                .Concat(
                    new ItemModel("1111A", "Item 1111a"),
                    new ItemModel("1111B", "Item 1111b"),
                    new ItemModel("1111C", "Item 1111c"),
                    new ItemModel("1111D", "Item 1111d"),
                    new ItemModel("1111E", "Item 1111e"),
                    new ItemModel("1111F", "Item 1111f"),
                    new ItemModel("1112A", "Item 1112a"),
                    new ItemModel("1112B", "Item 1112b"),
                    new ItemModel("1112C", "Item 1112c"),
                    new ItemModel("1112D", "Item 1112d"),
                    new ItemModel("1112E", "Item 1112e"),
                    new ItemModel("1112F", "Item 1112f"),
                    new ItemModel("1121A", "Item 1121a"),
                    new ItemModel("1121B", "Item 1121b"),
                    new ItemModel("1121C", "Item 1121c"),
                    new ItemModel("1121D", "Item 1121d"),
                    new ItemModel("1121E", "Item 1121e"),
                    new ItemModel("1121F", "Item 1121f"),
                    new ItemModel("1122A", "Item 1122a"),
                    new ItemModel("1122B", "Item 1122b"),
                    new ItemModel("1122C", "Item 1122c"),
                    new ItemModel("1122D", "Item 1122d"),
                    new ItemModel("1122E", "Item 1122e"),
                    new ItemModel("1122F", "Item 1122f"),
                    new ItemModel("1211A", "Item 1211a"),
                    new ItemModel("1211B", "Item 1211b"),
                    new ItemModel("1211C", "Item 1211c"),
                    new ItemModel("1211D", "Item 1211d"),
                    new ItemModel("1211E", "Item 1211e"),
                    new ItemModel("1211F", "Item 1211f"),
                    new ItemModel("1212A", "Item 1212a"),
                    new ItemModel("1212B", "Item 1212b"),
                    new ItemModel("1212C", "Item 1212c"),
                    new ItemModel("1212D", "Item 1212d"),
                    new ItemModel("1212E", "Item 1212e"),
                    new ItemModel("1212F", "Item 1212f"),
                    new ItemModel("1221A", "Item 1221a"),
                    new ItemModel("1221B", "Item 1221b"),
                    new ItemModel("1221C", "Item 1221c"),
                    new ItemModel("1221D", "Item 1221d"),
                    new ItemModel("1221E", "Item 1221e"),
                    new ItemModel("1221F", "Item 1221f"),
                    new ItemModel("1222A", "Item 1222a"),
                    new ItemModel("1222B", "Item 1222b"),
                    new ItemModel("1222C", "Item 1222c"),
                    new ItemModel("1222D", "Item 1222d"),
                    new ItemModel("1222E", "Item 1222e"),
                    new ItemModel("1222F", "Item 1222f"),
                    new ItemModel("2111A", "Item 2111a"),
                    new ItemModel("2111B", "Item 2111b"),
                    new ItemModel("2111C", "Item 2111c"),
                    new ItemModel("2111D", "Item 2111d"),
                    new ItemModel("2111E", "Item 2111e"),
                    new ItemModel("2111F", "Item 2111f"),
                    new ItemModel("2112A", "Item 2112a"),
                    new ItemModel("2112B", "Item 2112b"),
                    new ItemModel("2112C", "Item 2112c"),
                    new ItemModel("2112D", "Item 2112d"),
                    new ItemModel("2112E", "Item 2112e"),
                    new ItemModel("2112F", "Item 2112f"),
                    new ItemModel("2121A", "Item 2121a"),
                    new ItemModel("2121B", "Item 2121b"),
                    new ItemModel("2121C", "Item 2121c"),
                    new ItemModel("2121D", "Item 2121d"),
                    new ItemModel("2121E", "Item 2121e"),
                    new ItemModel("2121F", "Item 2121f"),
                    new ItemModel("2122A", "Item 2122a"),
                    new ItemModel("2122B", "Item 2122b"),
                    new ItemModel("2122C", "Item 2122c"),
                    new ItemModel("2122D", "Item 2122d"),
                    new ItemModel("2122E", "Item 2122e"),
                    new ItemModel("2122F", "Item 2122f"),
                    new ItemModel("2211A", "Item 2211a"),
                    new ItemModel("2211B", "Item 2211b"),
                    new ItemModel("2211C", "Item 2211c"),
                    new ItemModel("2211D", "Item 2211d"),
                    new ItemModel("2211E", "Item 2211e"),
                    new ItemModel("2211F", "Item 2211f"),
                    new ItemModel("2212A", "Item 2212a"),
                    new ItemModel("2212B", "Item 2212b"),
                    new ItemModel("2212C", "Item 2212c"),
                    new ItemModel("2212D", "Item 2212d"),
                    new ItemModel("2212E", "Item 2212e"),
                    new ItemModel("2212F", "Item 2212f"),
                    new ItemModel("2221A", "Item 2221a"),
                    new ItemModel("2221B", "Item 2221b"),
                    new ItemModel("2221C", "Item 2221c"),
                    new ItemModel("2221D", "Item 2221d"),
                    new ItemModel("2221E", "Item 2221e"),
                    new ItemModel("2221F", "Item 2221f"),
                    new ItemModel("2222A", "Item 2222a"),
                    new ItemModel("2222B", "Item 2222b"),
                    new ItemModel("2222C", "Item 2222c"),
                    new ItemModel("2222D", "Item 2222d"),
                    new ItemModel("2222E", "Item 2222e"),
                    new ItemModel("2222F", "Item 2222f"))
                .ToImmutableList();

            _subject = new BehaviorSubject<ImmutableList<ItemModel>>(model);

            _subject.ApplyOnCollection(this, Items,
                factory: () => new ItemVm(),
                syncer: (m, vm) => vm.ReadModel(m),
                trackBy: m => m.Uid,
                onRemove: vm => vm.Dispose());


            await Task.Delay(1000);

            SelectedIds.Add(Items[1]);
            SelectedIds.Add(Items[2]);

        }
    }
}
