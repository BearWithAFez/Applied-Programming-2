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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MazeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Simple block
            Point3D[] blokPunten = { new Point3D(0.5, 0.5, 0.5), new Point3D(-0.5, 0.5, 0.5), new Point3D(-0.5, -0.5, 0.5), new Point3D(0.5, -0.5, 0.5), new Point3D(0.5, 0.5, -0.5), new Point3D(-0.5, 0.5, -0.5), new Point3D(-0.5, -0.5, -0.5), new Point3D(0.5, -0.5, -0.5)};
            var blok = new Block(blokPunten, @"Resources\Hout.jpg");
            var blok2 = new Block(blokPunten, @"Resources\Hout.jpg");
            // Simple light
            var light = new AmbientLight()
            {
                Color = Colors.White
            };

            // Simple camera
            var camera = new PerspectiveCamera()
            {
                FieldOfView = 45,
                Position = new Point3D(0,2,3),
                LookDirection = new Vector3D(0,-2,-3),
                UpDirection = new Vector3D(0,1,0)
            };

            // Model elements
            var modelGrp = new Model3DGroup();
            modelGrp.Children.Add(blok.Model);
            modelGrp.Children.Add(blok2.Model);
            modelGrp.Children.Add(light);

            // Viewport
            var viewport = new Viewport3D()
            {
                Camera = camera,
                Height = 500,
                Width = 500
            };
            viewport.Children.Add(new ModelVisual3D() { Content = modelGrp });
            mainCanvas.Children.Add(viewport);

            // Canvas
            Canvas.SetTop(viewport, 0);
            Canvas.SetLeft(viewport, 0);

            // TransformGroup
            var transforms = new Transform3DGroup();
            blok.Model.Transform = transforms;
            var transforms2 = new Transform3DGroup();
            blok2.Model.Transform = transforms2;

            // Rotation
            var rotAxis = new AxisAngleRotation3D(new Vector3D(0,1,0),0);
            var rotation = new RotateTransform3D(rotAxis);
            var rotAngle = new DoubleAnimation()
            {
                From = 0,
                To = 360,
                Duration = new Duration(TimeSpan.FromSeconds(20.0)),
                RepeatBehavior = RepeatBehavior.Forever
            };
            transforms.Children.Add(rotation);
            transforms2.Children.Add(rotation);

            // Translate
            transforms2.Children.Add(new TranslateTransform3D(.77, 0, 0));
            transforms.Children.Add(new TranslateTransform3D(-.77, 0, 0));

            // Final linking (?)
            NameScope.SetNameScope(mainCanvas, new NameScope());
            mainCanvas.RegisterName("rotAxis", rotAxis);
            Storyboard.SetTargetName(rotAngle, "rotAxis");
            Storyboard.SetTargetProperty(rotAngle, new PropertyPath(AxisAngleRotation3D.AngleProperty));
            var rotCube = new Storyboard();
            rotCube.Children.Add(rotAngle);
            rotCube.Begin(mainCanvas);
        }
    }
}
