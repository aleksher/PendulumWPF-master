using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ikc5.TypeLibrary;

namespace PendulumWPF
{
    class ChartRepository : BaseNotifyPropertyChanged
    {
        private const int LineCount = 100;
        private readonly List<int> _lineCountList = new List<int>(LineCount);
        public IReadOnlyList<int> LineCountList => _lineCountList;
        public void AddLineCount(int newValue)
        {
            _lineCountList.Add(newValue);
            if (_lineCountList.Count > LineCount)
                _lineCountList.RemoveAt(0);

            OnPropertyChanged(nameof(LineCountList));
        }
    }
}
