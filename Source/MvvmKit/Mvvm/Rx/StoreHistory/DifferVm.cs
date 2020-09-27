using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmKit.Mvvm.Rx.StoreHistory
{
    public class DifferVm: BindableBase
    {
        private string _oldText = "";
        private string _newText = "";
        private SideBySideDiffModel _diff = null;

        #region Properties

        private List<DifferLineVm> _OldLines;
        public List<DifferLineVm> OldLines { get { return _OldLines; } set { SetProperty(ref _OldLines, value); } }

        private List<DifferLineVm> _NewLines;
        public List<DifferLineVm> NewLines { get { return _NewLines; } set { SetProperty(ref _NewLines, value); } }

        private bool _ShowAll;
        public bool ShowAll { get { return _ShowAll; } set { SetProperty(ref _ShowAll, value); } }


        private int _LinesAroundDiff;
        public int LinesAroundDiff { get { return _LinesAroundDiff; } set { SetProperty(ref _LinesAroundDiff, value); } }


        #endregion

        public DifferVm()
        {
            OldLines = new List<DifferLineVm>();
            NewLines = new List<DifferLineVm>();

            if (Exec.IsRunTime)
            {
                this
                    .ObservePropertyValues(vm => vm.ShowAll)
                    .Subscribe(_ => _invalidate())
                    .DisposedBy(this);

                this.ObservePropertyValues(vm => vm.LinesAroundDiff)
                    .Subscribe(_ => _invalidate())
                    .DisposedBy(this);
            }
        }

        public void ReadModel(string oldText, string newText)
        {
            _oldText = oldText ?? "";
            _newText = newText ?? "";

            _diff = SideBySideDiffBuilder.Diff(_oldText, _newText, true, false);
            _invalidate();
        }

        private void _invalidate()
        {
            if (_diff != null)
            {
                var diffIndices = _diff.OldText.Lines
                    .Select((line, index) => (line, index))
                    .Where(pair => pair.line.Type != ChangeType.Unchanged)
                    .SelectMany(pair => Enumerable.Range(pair.index - LinesAroundDiff, 2 * LinesAroundDiff + 1))
                    .Distinct()
                    .ToHashSet();

                OldLines = _diff.OldText.Lines
                    .Where((dp, index) => ShowAll || diffIndices.Contains(index))
                    .Select(line => new DifferLineVm().ReadModel(line))
                    .ToList();
                NewLines = _diff.NewText.Lines
                    .Where((dp, index) => ShowAll || diffIndices.Contains(index))
                    .Select(line => new DifferLineVm().ReadModel(line))
                    .ToList();
            }
            else
            {
                OldLines = new List<DifferLineVm>();
                NewLines = new List<DifferLineVm>();
            }

        }
    }
}
