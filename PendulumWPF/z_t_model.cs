using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ikc5.TypeLibrary;
using OxyPlot;

namespace PendulumWPF
{
    class z_t_model : BaseNotifyPropertyChanged
    {
        private readonly ChartRepository _chartRepository;

        public z_t_model(ChartRepository chartRepository)
        {
            chartRepository.ThrowIfNull(nameof(chartRepository));
            _chartRepository = chartRepository;
            chartRepository.PropertyChanged += ChartRepositoryPropertyChanged;
        }

        private void ChartRepositoryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!nameof(ChartRepository.LineCountList).Equals(e.PropertyName))
                return;

            OnPropertyChanged(nameof(CountList));
        }
        public IReadOnlyList<DataPoint> CountList =>
            _chartRepository.LineCountList.Select((value, index) => new DataPoint(index, value)).ToList();
    }
}
