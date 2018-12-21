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
            Model3DGroup pendulumObject = objReader.Read("pendulum.obj");
            scene.Content = pendulumObject;
            Create3DViewPort();

        }

        private void Create3DViewPort()
        {
            ObjReader pen = new ObjReader();
            Model3DGroup newpend = pen.Read("pendulum.obj");
            scene.Content = newpend;
            RotateTransform3D rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90));
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
        private double endT = 0.01;
        private double[] x0 = new double[] {0, 0, Math.PI * 2, 0 };
        private Transform3DGroup transform;
        private RotateTransform3D rotate;
        private TranslateTransform3D translate;
        private double i = 0;
        private void timerTick(object sender, EventArgs e)
        {
            var solve = WilberforcePendulum.GetOscillations(startT, deltaT, endT, x0);
            x0 = solve.x0;
            startT += deltaT;
            endT += deltaT;
            rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), solve.theta[0]*180 / Math.PI));
            //translate = new TranslateTransform3D(new Vector3D(0,0,solve.z[0]));
            transform = new Transform3DGroup();
            transform.Children.Add(rotate);
            //transform.Children.Add(translate);
            scene.Transform = transform;
            Console.WriteLine(solve.theta[0]);
        }

    }
}
    