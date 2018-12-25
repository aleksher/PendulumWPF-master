using System;
using System.Collections.Generic;
using System.Windows;
using HelixToolkit;
using HelixToolkit.Wpf;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using ODE;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;


namespace PendulumWPF
{
    public class z_t
    {
        public z_t()
        {
            MyModel = new PlotModel { Title = "" };
            GraphData.z_t.Title = "z";
            var z_axis = new LinearAxis();
            z_axis.Title = "z /m";
            z_axis.Key = "z";
            GraphData.z_t.YAxisKey = "z";
            GraphData.z_t.Color = OxyColor.FromRgb(0,0,255);
            MyModel.Axes.Add(z_axis);
            MyModel.Series.Add(GraphData.z_t);

            GraphData.theta_t.Title = "theta";
            var theta_axis = new LinearAxis();
            theta_axis.Title = "theta /rad";
            theta_axis.Key = "theta";
            theta_axis.Position = AxisPosition.Right;
            GraphData.theta_t.YAxisKey = "theta";
            GraphData.theta_t.Color = OxyColor.FromRgb(255,0,0);

            MyModel.Axes.Add(theta_axis);
            MyModel.Series.Add(GraphData.theta_t);

            var x_axis = new LinearAxis();
            x_axis.Position = AxisPosition.Bottom;
            x_axis.Title = "t /sec";

            MyModel.Axes.Add(x_axis);
        }

        public static PlotModel MyModel { get; private set; }
    }


public static class GraphData
    {
        public static LineSeries z_t =new LineSeries();
        public static LineSeries theta_t = new LineSeries();
        public static LineSeries theta_z = new LineSeries();
    }

    public partial class Pendulum : Window
    {
        public Pendulum()
        {
            InitializeComponent();
            ObjReader objReader = new ObjReader();
            Create3DViewPort();

        }

        Model3D pendulum;
        Model3D @base;
        Model3D stick90;
        Model3D stick;
        Model3D table;
        Model3D spring;

        private Transform3DGroup pendulumTransform;
        private Transform3DGroup springTransform;

        Model3DGroup system;
        private void Create3DViewPort()
        {
            system = new Model3DGroup();

            ModelImporter pendulumImporter = new ModelImporter();
            pendulumImporter.DefaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.DarkGoldenrod));
            pendulum = pendulumImporter.Load("pendulum.obj");
            pendulumTransform = new Transform3DGroup();
            pendulumTransform.Children.Add(new ScaleTransform3D(1.5,1.5,1.5));
            pendulumTransform.Children.Add(new TranslateTransform3D(0, 5 ,-3));
            pendulum.Transform = pendulumTransform;
            system.Children.Add(pendulum);

            ModelImporter baseImporter = new ModelImporter();
            baseImporter.DefaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.SteelBlue));
            @base = baseImporter.Load("tripod1d.obj");
            Transform3DGroup baseTransform = new Transform3DGroup();
            baseTransform.Children.Add(new ScaleTransform3D(0.25, 0.4, 0.25));
            baseTransform.Children.Add(new TranslateTransform3D(0, 0, -10));
            @base.Transform = baseTransform;
            system.Children.Add(@base);

            ModelImporter stick90Importer = new ModelImporter();
            stick90Importer.DefaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.SteelBlue));
            stick90 = stick90Importer.Load("tripod2d.obj");
            Transform3DGroup stick90Transform = new Transform3DGroup();
            stick90Transform.Children.Add(new ScaleTransform3D(0.25, 0.25, 0.25));
            stick90Transform.Children.Add(new TranslateTransform3D(0, -15, 5));
            stick90.Transform = stick90Transform;
            system.Children.Add(stick90);

            ModelImporter stickImporter = new ModelImporter();
            stickImporter.DefaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.SteelBlue));
            stick = stickImporter.Load("tripod3d.obj");
            Transform3DGroup stickTransform = new Transform3DGroup();
            stickTransform.Children.Add(new ScaleTransform3D(0.25, 0.4, 0.25));
            stickTransform.Children.Add(new TranslateTransform3D(0, -34, 12));
            stick.Transform = stickTransform;
            system.Children.Add(stick);

            ModelImporter tableImporter = new ModelImporter();
            tableImporter.DefaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.BurlyWood));
            table = tableImporter.Load("table.obj");
            Transform3DGroup tableTransform = new Transform3DGroup();
            tableTransform.Children.Add(new ScaleTransform3D(0.03, 0.03, 0.03));
            tableTransform.Children.Add(new TranslateTransform3D(15, -15, 15));
            table.Transform = tableTransform;
            system.Children.Add(table);

            ModelImporter springImporter = new ModelImporter();
            springImporter.DefaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Silver));
            spring = springImporter.Load("spring.obj");

            springTransform = new Transform3DGroup();
            springTransform.Children.Add(new TranslateTransform3D(0, 5, -11.4));
            springTransform.Children.Add(new ScaleTransform3D(1, 1, -1));
            var cutLength = stick.Bounds.Z - (pendulum.Bounds.Z + pendulum.Bounds.SizeZ);
            var scale = cutLength / spring.Bounds.SizeZ;
            springTransform.Children.Add(new ScaleTransform3D(new Vector3D(1, 1, scale*1.03), new Point3D(0, 5, stick.Bounds.Z)));

            spring.Transform = springTransform;

            system.Children.Add(spring);

            scene.Content = system;
        }

        private DispatcherTimer timerGraph;
        private OXYPlotTest graph;
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // открывается из "Сохранить скриншот"
            graph = new OXYPlotTest();
            graph.Show();

            timerGraph = new DispatcherTimer();
            timerGraph.Tick += timerGraphTick;
            timerGraph.Interval = new TimeSpan(0, 0, 0, 0, 30);
            timerGraph.Start();

        }

        private DispatcherTimer timerPendulum;
        private void Start_OnClick(object sender, RoutedEventArgs e)
        {
            timerPendulum = new DispatcherTimer();
            timerPendulum.Tick += timerTick;
            timerPendulum.Interval = new TimeSpan(0, 0, 0, 0, 30);
            timerPendulum.Start();
        }

        private bool graphFin = true;
        private void timerGraphTick(object sender, EventArgs e)
        {
            if (graphFin)
            {
                graphFin = false;
                z_t.MyModel.InvalidatePlot(true);
                graphFin = true;
            }
        }

        private double startT = 0;
        private double deltaT = 0.05;
        private double endT = 0.1;
        private double[] y0 = {0, 0, 0, 0 };

        private RotateTransform3D rotate;
        private ScaleTransform3D scaleTransform;

        private double[,] solve;
        private double cutLength;
        private double scale;

        private int count = 0;
        private bool pendFin = true;

        private double mass = 0.1;
        private double k = 1;
        private double nutMass = 0.01;

        private void timerTick(object sender, EventArgs e)
        {
            if (!paused)
            {

                //solve[i,0] - t, solve[i, 1] - z, solve[i, 2] - zdot, solve[i, 3] - theta, solve[i, 4] - thetadot 
                solve = WilberforcePendulum.GetOscillations(startT, deltaT, endT, y0, mass, k, nutMass);
                y0[0] = solve[1, 1];
                y0[1] = solve[1, 2];
                y0[2] = solve[1, 3];
                y0[3] = solve[1, 4];

                startT += deltaT;
                endT += deltaT;

                // преобразование маятника
                rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1),
                    0.8*solve[0, 3] * 180 / Math.PI));
                pendulumTransform.Children.Clear(); // очистим предыдущие изменения
                pendulumTransform.Children.Add(rotate);
                pendulumTransform.Children.Add(new ScaleTransform3D(1.5, 1.5, 1.5));
                pendulumTransform.Children.Add(new TranslateTransform3D(0, 5, -3 + 30 * solve[0, 1]));
                pendulum.Transform = pendulumTransform;

                // преобразование пружины
                cutLength = stick.Bounds.Z - (pendulum.Bounds.Z + pendulum.Bounds.SizeZ);
                scale = cutLength / spring.Bounds.SizeZ;
                scaleTransform =
                    new ScaleTransform3D(new Vector3D(1, 1, scale * 1.03), new Point3D(0, 5, stick.Bounds.Z));
                springTransform.Children.Add(scaleTransform);

                // формирование списка данных для графиков
                if (count++ > 2)
                {
                    GraphData.z_t.Points.Add(new DataPoint(solve[0, 0], solve[0, 1]));
                    GraphData.theta_t.Points.Add(new DataPoint(solve[0, 0], solve[0, 3]));
                    GraphData.theta_z.Points.Add(new DataPoint(solve[0, 1], solve[0, 3]));
                    count = 0;
                }

                if (GraphData.theta_z.Points.Count > 700)
                {
                    GraphData.z_t.Points.RemoveAt(0);
                    GraphData.theta_t.Points.RemoveAt(0);
                    GraphData.theta_z.Points.RemoveAt(0);
                }

            }
        }

        // Масса
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mass = e.NewValue;
        }

        // Наальное смещение
        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            y0[0] = e.NewValue / 100;
            pendulumTransform.Children.Clear();
            pendulumTransform.Children.Add(new ScaleTransform3D(1.5, 1.5, 1.5));
            pendulumTransform.Children.Add(new TranslateTransform3D(0, 5, -3 + y0[0]*30));

            //пружину
            cutLength = stick.Bounds.Z - (pendulum.Bounds.Z + pendulum.Bounds.SizeZ);
            scale = cutLength / spring.Bounds.SizeZ;
            scaleTransform =
                new ScaleTransform3D(new Vector3D(1, 1, scale * 1.03), new Point3D(0, 5, stick.Bounds.Z));
            springTransform.Children.Add(scaleTransform);
        }

        // Начальный угол
        private void Slider_ValueChanged_2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            y0[2] = e.NewValue / 180 * Math.PI ;
        }

        // Масса гайки
        private void Slider_ValueChanged_3(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            nutMass = e.NewValue;
        }

        private void Slider_ValueChanged_4(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            k = e.NewValue;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Reference p = new Reference();
            p.Show();
        }

        private bool paused = false;
        private void Pause_OnClick(object sender, RoutedEventArgs e)
        {
            paused = true;
        }

        private void Stop_OnClick(object sender, RoutedEventArgs e)
        {
            if (paused)
                paused = false;
            else
            {
                timerPendulum.Stop();

                y0 = new double[] {0, 0, 0, 0};
                startT = 0;
                endT = 0.1;
                GraphData.z_t.Points.Clear();
                GraphData.theta_t.Points.Clear();

                pendulumTransform.Children.Clear();
                pendulumTransform.Children.Add(new ScaleTransform3D(1.5, 1.5, 1.5));
                pendulumTransform.Children.Add(new TranslateTransform3D(0, 5, -3));

                if (graph != null && graph.IsEnabled)
                {
                    timerGraph.Stop();
                    graph.Close();
                    z_t.MyModel.InvalidatePlot(true);
                    z_t.MyModel.Series.Clear();

                }
            }
        }
    }

}
    