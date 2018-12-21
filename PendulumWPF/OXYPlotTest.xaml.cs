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
using System.Windows.Shapes;

using OxyPlot;
using OxyPlot.Series;

namespace PendulumWPF
{
    /// <summary>
    /// Interaction logic for OXYPlotTest.xaml
    /// </summary>
    public partial class OXYPlotTest : Window
    {
        public OXYPlotTest()
        {
            InitializeComponent();
        }
    }

    public class MainViewModel
    {
        public MainViewModel()
        {
            Title = "Example 2";
        }

        public string Title { get; private set; }
        public IList<DataPoint> Points { get; private set; }
    }
}
