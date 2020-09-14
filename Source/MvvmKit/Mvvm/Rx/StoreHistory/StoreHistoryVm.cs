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
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace MvvmKit.Mvvm.Rx.StoreHistory
{
    public class StoreHistoryVm : ComponentBase
    {
        private ImmutableDictionary<object, EnsureSessionHistory> _ensureSessions;

        #region Properties

        private ObservableCollection<HistoryRecordVm> _Records;
        public ObservableCollection<HistoryRecordVm> Records { get { return _Records; } set { SetProperty(ref _Records, value); } }

        private int _SelectedItem;
        public int SelectedItem { get { return _SelectedItem; } set { SetProperty(ref _SelectedItem, value); } }

        private List<JToken> _Action;
        public List<JToken> Action { get { return _Action; } set { SetProperty(ref _Action, value); } }

        private List<JToken> _State;
        public List<JToken> State { get { return _State; } set { SetProperty(ref _State, value); } }

        private DifferVm _Differ;
        public DifferVm Differ { get { return _Differ; } set { SetProperty(ref _Differ, value); } }


        private ObservableCollection<EnsureItemVm> _EnsureItems;
        public ObservableCollection<EnsureItemVm> EnsureItems { get { return _EnsureItems; } set { SetProperty(ref _EnsureItems, value); } }

        private int _SelectedEnsureItem;
        public int SelectedEnsureItem { get { return _SelectedEnsureItem; } set { SetProperty(ref _SelectedEnsureItem, value); } }

        private List<JToken> _EnsureItemState;
        public List<JToken> EnsureItemState { get { return _EnsureItemState; } set { SetProperty(ref _EnsureItemState, value); } }

        private DifferVm _EnsureDiffer;
        public DifferVm EnsureDiffer { get { return _EnsureDiffer; } set { SetProperty(ref _EnsureDiffer, value); } }


        #endregion

        public StoreHistoryVm()
        {
            Records = new ObservableCollection<HistoryRecordVm>();
            EnsureItems = new ObservableCollection<EnsureItemVm>();
            Differ = new DifferVm();
            EnsureDiffer = new DifferVm();
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

        private List<JToken> _pure_createJsonHierarchy(object obj)
        {
            var res = new List<JToken>();

            try
            {
                var token = obj.ToJsonToken();
                if (token != null) res.Add(token);
            }
            catch { }

            return res;
        }

        private HistoryRecord _pure_getSelectedHistoryRecord(ImmutableList<HistoryRecord> records, int index)
        {
            if ((index < 0) || (index >= records.Count))
                return null;

            return records[index];
        }

        private EnsureSessionHistory _pure_getEnsureSessionHistory(HistoryRecord record, ImmutableDictionary<object, EnsureSessionHistory> ensures)
        {
            if (_ensureSessions.TryGetValue(record.Action, out var session))
            {
                return session;
            } else
            {
                return null;
            }
        }

        private EnsureHistoryItem _pure_getSelectedEnsureItem(EnsureSessionHistory session, int index)
        {
            if (session == null) return null;
            if ((index < 0) || (index >= session.Items.Count)) return null;

            return session.Items[index];
        }


        public void ConnectToStore<T>(ReduxStore<T> store)
            where T: class, IImmutable, new()
        {
            var rootSelector = Selectors.CreateSelector((T state) => state);

            var onHistory = store.ObserveHistory()
                .WithLatestFrom(store.Select(rootSelector), (history, state) => (history, state))
                .Select(pair => _selectHistoryRecords(pair.history, pair.state));

            var onLatestEnsureHistory = EnsureManager
                .GetHistory()
                .Select(history => history.ToImmutableDictionary(session => session.Action));

            onLatestEnsureHistory
                .Subscribe(val => _ensureSessions = val)
                .DisposedBy(this);

            MvvmRx.ApplyOnCollection(onHistory, this, Records,
                factory: Resolver.Resolve<HistoryRecordVm>,
                syncer: (model, vm) => vm.ReadModel(model, _ensureSessions),
                onRemove: vm => vm.Dispose());

            var onCurrentHistoryRecord = Observable.CombineLatest(
                onHistory,
                MvvmRx.ObservePropertyValues(this, x => x.SelectedItem),
                (records, index) => (records, index))
                .Select(pair => _pure_getSelectedHistoryRecord(pair.records, pair.index))
                .Where(record => record != null);

            onCurrentHistoryRecord
                .Select(record => _pure_createJsonHierarchy(record.Action))
                .ApplyOnProperty(this, x => x.Action);

            onCurrentHistoryRecord
                .Select(record => _pure_createJsonHierarchy(record.NextState))
                .ApplyOnProperty(this, x => x.State);

            onCurrentHistoryRecord
                .Subscribe(record => Differ.ReadModel(record.PreviousState.ToJson(), record.NextState.ToJson()))
                .DisposedBy(this);

            var onEnsureSession = Observable.CombineLatest(onLatestEnsureHistory, onCurrentHistoryRecord,
                (history, record) => _pure_getEnsureSessionHistory(record, history));

            onEnsureSession
                .Select(session => session?.Items
                                          ?.Select(item => Resolver.Resolve<EnsureItemVm>().ReadModel(item))
                                          ?.ToObservableCollection()
                                          ?? new ObservableCollection<EnsureItemVm>()
                                          )
                .ApplyOnProperty(this, x => x.EnsureItems);

            var onEnsureItem = Observable.CombineLatest(
                onEnsureSession,
                MvvmRx.ObservePropertyValues(this, x => x.SelectedEnsureItem),
                (session, index) => (session, index))
                .Select(pair => _pure_getSelectedEnsureItem(pair.session, pair.index));

            onEnsureItem.Select(item => _pure_createJsonHierarchy(item?.Context.Entity))
                .ApplyOnProperty(this, x => x.EnsureItemState);

            onEnsureItem
                .Subscribe(item => EnsureDiffer.ReadModel(item?.Before.ToJson(), item?.After.ToJson()))
                .DisposedBy(this);

        }

    }
}
