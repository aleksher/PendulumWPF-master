using System;
using System.Windows;
using HelixToolkit;
using HelixToolkit.Wpf;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using ODE;


namespace PendulumWPF
{
    public partial class Pendulum : Window
    {
        public Pendulum()
        {
            InitializeComponent();
            ObjReader objReader = new ObjReader();
            Create3DViewPort();

        }

        Model3D pendulum;
        Model3D tripod;
        Model3D table;
        Model3DGroup system;
        private void Create3DViewPort()
        {
            system = new Model3DGroup();

            ModelImporter pendulumImporter = new ModelImporter();
            pendulumImporter.DefaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.DarkMagenta));
            pendulum = pendulumImporter.Load("pendulum90ver2.obj");
            Transform3DGroup pendTransform = new Transform3DGroup();
            pendTransform.Children.Add(new ScaleTransform3D(1.5,1.5,1.5));
            pendTransform.Children.Add(new TranslateTransform3D(0, 5 ,-3));
            pendulum.Transform = pendTransform;
            system.Children.Add(pendulum);

            ModelImporter tripodImporter = new ModelImporter();
            tripodImporter.DefaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.CornflowerBlue));
            tripod = tripodImporter.Load("tripod.obj");
            Transform3DGroup tripodTransform = new Transform3DGroup();
            tripodTransform.Children.Add(new ScaleTransform3D(0.25, 0.25, 0.25));
            tripodTransform.Children.Add(new TranslateTransform3D(0, 0, -10));
            tripod.Transform = tripodTransform;
            system.Children.Add(tripod);

            ModelImporter tableImporter = new ModelImporter();
            tableImporter.DefaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.BurlyWood));
            table = tableImporter.Load("table.obj");
            Transform3DGroup tableTransform = new Transform3DGroup();
            tableTransform.Children.Add(new ScaleTransform3D(0.03, 0.03, 0.03));
            tableTransform.Children.Add(new TranslateTransform3D(15, -15, 15));
            table.Transform = tableTransform;
            system.Children.Add(table);

            scene.Content = system;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // открывается из "Сохранить скриншот"
            var window = new System.Windows.Window();
            window.Content = new theta_plot();
            window.Show();
        }

        private void Start_OnClick(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += timerTick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            timer.Start();

            var graph = new OXYPlotTest();
            graph.Show();
        }

        private double startT = 0;
        private double deltaT = 0.01;
        private double endT = 0.02;
        private double[] y0 = new double[] {0, 0, Math.PI * 2, 0 };
        private Transform3DGroup transform;
        private RotateTransform3D rotate;

        private void timerTick(object sender, EventArgs e)
        {
            //solve[i,0] - t, solve[i, 1] - z, solve[i, 2] - zdot, solve[i, 3] - theta, solve[i, 4] - thetadot 
            double[,] solve = WilberforcePendulum.GetOscillations(startT, deltaT, endT, y0);
            y0 = new[] {solve[1, 1], solve[1, 2], solve[1, 3], solve[1, 4]};
         
            startT += deltaT;
            endT += deltaT;

            transform = new Transform3DGroup();
            rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), solve[0, 3] * 180 / Math.PI));
            transform.Children.Add(rotate);
            transform.Children.Add(new TranslateTransform3D(0, 3, -3 + 20 * solve[0, 1]));
            transform.Children.Add(new ScaleTransform3D(1.5, 1.5, 1.5));
            pendulum.Transform = transform;
        }
    }
}
    