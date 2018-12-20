using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
using ODE;
namespace PendulumWPF
{
    /// <summary>
    /// Interaction logic for z_plot.xaml
    /// </summary>
    public partial class z_plot : UserControl
    {
        public z_plot()
        {
            InitializeComponent();
            var solution = WilberforcePendulum.GetOscillations(0, 0.1, 40);
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "z/m",
                    
                    Values = new ChartValues<double>(solution.z),
                    PointGeometry = null
                }
            };
            YFormatter = value => (value / 10).ToString();
            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public Func<double, string> YFormatter { get; set; }
    }
}
