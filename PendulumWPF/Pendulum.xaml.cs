using System;
using System.Windows;
using HelixToolkit;
using HelixToolkit.Wpf;
using System.IO;
using System.Linq;
using System.Windows.Media.Media3D;
using  ODE;

namespace PendulumWPF
{
    public partial class Pendulum : Window
    {
        public Pendulum()
        {
            InitializeComponent();
            Model3D pendulum;
            ModelImporter importer = new ModelImporter();
            pendulum = importer.Load("pendulum.obj");
            ObjReader pen = new ObjReader();
            Model3DGroup newpend = pen.Read("pendulum.obj");

            our_Model.Content = newpend;
            Create3DViewPort();

        }

        private void Create3DViewPort()
        {
            ObjReader pen = new ObjReader();
            Model3DGroup newpend = pen.Read("pendulum.obj");
            our_Model.Content = newpend;
            RotateTransform3D rotate = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90));
            our_Model.Transform = rotate;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var window = new System.Windows.Window();
            window.Content = new theta_plot();
            window.Show();
        }
    }
}
    