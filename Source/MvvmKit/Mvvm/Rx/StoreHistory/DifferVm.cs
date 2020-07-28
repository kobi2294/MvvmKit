using DiffPlex.DiffBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Mvvm.Rx.StoreHistory
{
    public class DifferVm: BindableBase
    {
        #region Properties

        private List<DifferLineVm> _OldLines;
        public List<DifferLineVm> OldLines { get { return _OldLines; } set { SetProperty(ref _OldLines, value); } }

        private List<DifferLineVm> _NewLines;
        public List<DifferLineVm> NewLines { get { return _NewLines; } set { SetProperty(ref _NewLines, value); } }

        #endregion

        public void ReadModel(string oldText, string newText)
        {
            var diff = SideBySideDiffBuilder.Diff(oldText, newText, true, false);

            OldLines = diff.OldText.Lines.Select(line => new DifferLineVm().ReadModel(line)).ToList();
            NewLines = diff.NewText.Lines.Select(line => new DifferLineVm().ReadModel(line)).ToList();
        }
    }
}
