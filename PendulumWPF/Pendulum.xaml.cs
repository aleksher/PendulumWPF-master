using System;
using System.Windows;
using HelixToolkit;
using HelixToolkit.Wpf;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using  ODE;

namespace PendulumWPF
{
    public partial class Pendulum : Window
    {
        public Pendulum()
        {
            InitializeComponent();
            ObjReader objReader = new ObjReader();
            //Model3DGroup pendulumObject = objReader.Read("pendulum.obj");
            //scene.Content = pendulumObject;
            Create3DViewPort();

        }

        private void Create3DViewPort()
        {
            ObjReader pen = new ObjReader();
            Model3DGroup pendulum = pen.Read("pendulum90 (3).obj");
            scene.Content = pendulum;
            scene.Transform = new ScaleTransform3D(2,2,2);
            //RotateTransform3D rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90));
            //scene.Transform = rotate;
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
        }

        private double startT = 0;
        private double deltaT = 0.01;
        private double endT = 0.02;
        private double[] y0 = new double[] {0, 0, Math.PI * 2, 0 };
        private Transform3DGroup transform;
        private RotateTransform3D rotate;
        private TranslateTransform3D translate;
        private double i = 0;
        private void timerTick(object sender, EventArgs e)
        {
            //solve[i,0] - t, solve[i, 1] - z, solve[i, 2] - zdot, solve[i, 3] - theta, solve[i, 4] - thetadot 
            double[,] solve = WilberforcePendulum.GetOscillations(startT, deltaT, endT, y0);
            y0 = new[] {solve[1, 1], solve[1, 2], solve[1, 3], solve[1, 4]};
         
            startT += deltaT;
            endT += deltaT;
            rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), solve[0, 3] * 180 / Math.PI));
            translate = new TranslateTransform3D(0,0,20*solve[0, 1]);
            transform = new Transform3DGroup();
            transform.Children.Add(rotate);
            transform.Children.Add(translate);
            transform.Children.Add(new ScaleTransform3D(2, 2, 2));
            scene.Transform = transform;
            Console.WriteLine(solve[0, 1]);
        }

    }
}
    