using MvvmKit;
using Newtonsoft.Json.Linq;
using ReduxSimple;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace MvvmKit.Mvvm.Rx.StoreHistory
{
    public class StoreHistoryVm : ComponentBase
    {
        #region Properties

        private ObservableCollection<HistoryRecordVm> _Records;
        public ObservableCollection<HistoryRecordVm> Records { get { return _Records; } set { SetProperty(ref _Records, value); } }

        private int _SelectedItem;
        public int SelectedItem { get { return _SelectedItem; } set { SetProperty(ref _SelectedItem, value); } }


        private List<JToken> _Action;
        public List<JToken> Action { get { return _Action; } set { SetProperty(ref _Action, value); } }


        private List<JToken> _State;
        public List<JToken> State { get { return _State; } set { SetProperty(ref _State, value); } }


        private string _OldState;
        public string OldState { get { return _OldState; } set { SetProperty(ref _OldState, value); } }


        private string _NewState;
        public string NewState { get { return _NewState; } set { SetProperty(ref _NewState, value); } }



        #endregion

        public StoreHistoryVm()
        {
            Records = new ObservableCollection<HistoryRecordVm>();
        }

        [InjectionMethod]
        public void Inject()
        {
        }

        protected override async Task OnInitialized(object param)
        {
            await base.OnInitialized(param);

        }

        private ImmutableList<HistoryRecord> _selectHistoryRecords<T>(ReduxHistory<T> history, T currentState)
            where T: class, IImmutable, new()
        {
            var mementosOrderedByDate = history.PreviousStates
                                                .OrderBy(memento => memento.Date)
                                                .ToList();

            var res = mementosOrderedByDate.Select((memento, index) => new HistoryRecord {
                Date = memento.Date,
                Action = memento.Action,
                PreviousState = memento.PreviousState,
                NextState = index < mementosOrderedByDate.Count - 1
                            ? mementosOrderedByDate[index + 1].PreviousState
                            : currentState
            }).ToImmutableList();

            return res;
        }

        private List<JToken> _createJsonHierarchy(object obj)
        {
            var res = new List<JToken>();

            try
            {
                var token = JToken.FromObject(obj);
                if (token != null) res.Add(token);
            }
            catch { }

            return res;
        }

        private void _createJsonHierarchies(ImmutableList<HistoryRecord> records, int index)
        {
            if ((index < 0) || (index >= records.Count))
                return;

            var record = records[index];
            Action = _createJsonHierarchy(record.Action);
            State = _createJsonHierarchy(record.NextState);
            OldState = record.PreviousState.ToJson();
            NewState = record.NextState.ToJson();
        }

        public void ConnectToStore<T>(ReduxStore<T> store)
            where T: class, IImmutable, new()
        {
            var rootSelector = Selectors.CreateSelector((T state) => state);

            var observable = store.ObserveHistory()
                .WithLatestFrom(store.Select(rootSelector), (history, state) => (history, state))
                .Select(pair => _selectHistoryRecords(pair.history, pair.state));

            MvvmRx.ApplyOnCollection(observable, this, Records,
                factory: Resolver.Resolve<HistoryRecordVm>,
                syncer: (model, vm) => vm.ReadModel(model),
                onRemove: vm => vm.Dispose());

            Observable.CombineLatest(
                observable,               
                MvvmRx.ObservePropertyValues(this, x => x.SelectedItem),
                (records, index) => (records, index))
                .Subscribe(data =>
                {
                    _createJsonHierarchies(data.records, data.index);

                }).DisposedBy(this);
        }

    }
}
