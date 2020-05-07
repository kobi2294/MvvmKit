using MvvmKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
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


        private ObservableCollection<string> _SelectedIds;
        public ObservableCollection<string> SelectedIds { get { return _SelectedIds; } set { SetProperty(ref _SelectedIds, value); } }

        #endregion

        #region Commands

        public IRxCommand<IEnumerable<string>> SelectionCommand { get; }

        public IRxCommand RandomFiveCommand { get; }

        public IRxCommand<IEnumerable> RemoveSelected { get; }

        public IRxCommand ResetCommand { get; }

        #endregion

        public SelectManyDemoVm()
        {
            Items = new ObservableCollection<ItemVm>().AllDisposedBy(this);
            SelectedIds = new ObservableCollection<string>  { "2", "5", "8", "1111", "11115", "22229", "StamItem" };
            Initialize();
            SelectionCommand = MvvmRx.CreateCommand<IEnumerable<String>>(this);
            RandomFiveCommand = MvvmRx.CreateCommand(this);
            RemoveSelected = MvvmRx.CreateCommand<IEnumerable>(this);
            ResetCommand = MvvmRx.CreateCommand(this);

            SelectionCommand.Select(ienum => ienum.ToObservableCollection()).ApplyOnProperty(this, x => x.SelectedIds);
            RandomFiveCommand.Subscribe(_ =>
            {
                var rnd = new Random();
                var items = Enumerable.Range(0, 5).Select(__ => Items[rnd.Next(Items.Count)].Uid);
                SelectedIds.AddRange(items);
            }).DisposedBy(this);

            RemoveSelected.Subscribe(x =>
            {
                foreach (var item in x.OfType<string>().ToArray())
                {
                    SelectedIds.Remove(item);
                }

            }).DisposedBy(this);

            ResetCommand.Subscribe(_ =>
            {
                var rnd = new Random();
                var ids = Items.Select(x => x.Uid).Where(x => rnd.Next(40) < 2).ToObservableCollection();
                SelectedIds = ids;
            });
            
        }

        public void Initialize()
        {
            var model = Enumerable
                .Range(0, 10000)
                .Select(i => new ItemModel(uid: i.ToString(), displayName: "Item " + i))
                .ToImmutableList();

            _subject = new BehaviorSubject<ImmutableList<ItemModel>>(model);

            _subject.ApplyOnCollection(this, Items,
                factory: () => new ItemVm(),
                syncer: (m, vm) => vm.ReadModel(m),
                trackBy: m => m.Uid,
                onRemove: vm => vm.Dispose());

        }
    }
}
