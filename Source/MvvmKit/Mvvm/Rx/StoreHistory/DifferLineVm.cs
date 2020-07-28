using DiffPlex.DiffBuilder.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Mvvm.Rx.StoreHistory
{
    public class DifferLineVm: BindableBase
    {
        #region Properties

        private string _Position;
        public string Position { get { return _Position; } set { SetProperty(ref _Position, value); } }

        private ChangeType _ChangeType;
        public ChangeType ChangeType { get { return _ChangeType; } set { SetProperty(ref _ChangeType, value); } }

        private string _Text;
        public string Text { get { return _Text; } set { SetProperty(ref _Text, value); } }

        public DifferLineVm ReadModel(DiffPiece line)
        {
            Position = line.Position?.ToString() ?? "";
            Text = line.Text;
            ChangeType = line.Type;
            return this;
        }

        #endregion
    }
}
