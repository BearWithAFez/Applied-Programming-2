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
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;

namespace Cube
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //Window_Loaded(this,new RoutedEventArgs());
        }

        MeshGeometry3D MCube()
        {
            MeshGeometry3D cube = new MeshGeometry3D();
            Point3DCollection corners = new
                                   Point3DCollection();
            corners.Add(new Point3D(0.5, 0.5, 0.5));
            corners.Add(new Point3D(-0.5, 0.5, 0.5));
            corners.Add(new Point3D(-0.5, -0.5, 0.5));
            corners.Add(new Point3D(0.5, -0.5, 0.5));
            corners.Add(new Point3D(0.5, 0.5, -0.5));
            corners.Add(new Point3D(-0.5, 0.5, -0.5));
            corners.Add(new Point3D(-0.5, -0.5, -0.5));
            corners.Add(new Point3D(0.5, -0.5, -0.5));
            cube.Positions = corners;

            Int32[] indices ={
            //front
                0,1,2,
                0,2,3,
            //back
               4,7,6,
                4,6,5,
              //Right
                4,0,3,
                4,3,7,
              //Left
                1,5,6,
                 1,6,2,
              //Top
                 1,0,4,
                 1,4,5,
              //Bottom
                 2,6,7,
                 2,7,3
              };

            Int32Collection Triangles =
                                  new Int32Collection();
            foreach (Int32 index in indices)
            {
                Triangles.Add(index);
            }
            cube.TriangleIndices = Triangles;

            //adding TextureCoordinates

            PointCollection woodpoints = new
                                   PointCollection();

            woodpoints.Add(new Point(1, 0));
            woodpoints.Add(new Point(0, 0));
            woodpoints.Add(new Point(0, 1));
            woodpoints.Add(new Point(1, 1));

            woodpoints.Add(new Point(1, 0));
            woodpoints.Add(new Point(0, 0));
            woodpoints.Add(new Point(0, 1));
            woodpoints.Add(new Point(1, 1));


            woodpoints.Add(new Point(0, 1));
            woodpoints.Add(new Point(0, 0));
            woodpoints.Add(new Point(1, 0));
            woodpoints.Add(new Point(1, 1));


            woodpoints.Add(new Point(0, 1));
            woodpoints.Add(new Point(0, 0));
            woodpoints.Add(new Point(1, 0));
            woodpoints.Add(new Point(1, 1));


            woodpoints.Add(new Point(1, 0));
            woodpoints.Add(new Point(0, 0));
            woodpoints.Add(new Point(0, 1));
            woodpoints.Add(new Point(1, 1));

            woodpoints.Add(new Point(1, 0));
            woodpoints.Add(new Point(0, 0));
            woodpoints.Add(new Point(0, 1));
            woodpoints.Add(new Point(1, 1));


            cube.TextureCoordinates = woodpoints;

            return cube;
        }

        private void Window_Loaded(object sender,
                                  RoutedEventArgs e)
        {
            GeometryModel3D Cube1 =
                                 new GeometryModel3D();
            MeshGeometry3D cubeMesh = MCube();
            Cube1.Geometry = cubeMesh;

            //Cube1.Material = new DiffuseMaterial(
            //          new SolidColorBrush(Colors.Azure));

            // Make the surface's material using an image brush.
            ImageBrush colors_brush = new ImageBrush();
            colors_brush.ImageSource =
                new BitmapImage(new Uri("wood.jpg", UriKind.Relative));
            DiffuseMaterial colors_material =
                new DiffuseMaterial(colors_brush);

            Cube1.Material = colors_material;




            DirectionalLight DirLight1 =
                                new DirectionalLight();
            DirLight1.Color = Colors.White;
            DirLight1.Direction =
                              new Vector3D(-1, -1, -1);

            PerspectiveCamera Camera1 =
                               new PerspectiveCamera();
            Camera1.FarPlaneDistance = 20;
            Camera1.NearPlaneDistance = 1;
            Camera1.FieldOfView = 45;
            Camera1.Position = new Point3D(2, 2, 3);
            Camera1.LookDirection =
                              new Vector3D(-2, -2, -3);
            Camera1.UpDirection =
                                 new Vector3D(0, 1, 0);

            Model3DGroup modelGroup =
                                    new Model3DGroup();
            modelGroup.Children.Add(Cube1);
            modelGroup.Children.Add(DirLight1);
            ModelVisual3D modelsVisual =
                                   new ModelVisual3D();
            modelsVisual.Content = modelGroup;

            Viewport3D myViewport = new Viewport3D();
            myViewport.Camera = Camera1;
            myViewport.Children.Add(modelsVisual);
            Canvas1.Children.Add(myViewport);

           // this.Content = myViewport;

            myViewport.Height = 500;
            myViewport.Width = 500;
            Canvas.SetTop(myViewport, 0);
            Canvas.SetLeft(myViewport, 0);
            this.Width = myViewport.Width;
            this.Height = myViewport.Height;

            AxisAngleRotation3D axis =
                         new AxisAngleRotation3D(
                           new Vector3D(0, 1, 0), 0);
            RotateTransform3D Rotate =
                         new RotateTransform3D(axis);
            Cube1.Transform = Rotate;
            DoubleAnimation RotAngle =
                               new DoubleAnimation();
            RotAngle.From = 0;
            RotAngle.To = 360;
            RotAngle.Duration = new Duration(
                         TimeSpan.FromSeconds(20.0));
            RotAngle.RepeatBehavior =
                             RepeatBehavior.Forever;
            NameScope.SetNameScope(Canvas1,
                                   new NameScope());
            Canvas1.RegisterName("cubeaxis", axis);
            Storyboard.SetTargetName(RotAngle,
                                       "cubeaxis");
            Storyboard.SetTargetProperty(RotAngle,
             new PropertyPath(
                AxisAngleRotation3D.AngleProperty));
            Storyboard RotCube = new Storyboard();
            RotCube.Children.Add(RotAngle);
            RotCube.Begin(Canvas1);
        }
    }
}
