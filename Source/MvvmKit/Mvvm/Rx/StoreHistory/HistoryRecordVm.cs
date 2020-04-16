using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Mvvm.Rx.StoreHistory
{
    public class HistoryRecordVm: BindableBase
    {
        #region Properties

        private DateTime _Date;
        public DateTime Date { get { return _Date; } set { SetProperty(ref _Date, value); } }

        private string _Caption;
        public string Caption { get { return _Caption; } set { SetProperty(ref _Caption, value); } }

        private object _ActionDetails;
        public object ActionDetails { get { return _ActionDetails; } set { SetProperty(ref _ActionDetails, value); } }

        private object _PreviousState;
        public object PreviousState { get { return _PreviousState; } set { SetProperty(ref _PreviousState, value); } }

        private object _NextState;
        public object NextState { get { return _NextState; } set { SetProperty(ref _NextState, value); } }

        #endregion

        public HistoryRecordVm ReadModel(HistoryRecord model)
        {
            Date = model.Date;
            Caption = model.Action.GetType().Name;
            ActionDetails = model.Action;
            PreviousState = model.PreviousState;
            NextState = model.NextState;
            return this;
        }
    }
}
