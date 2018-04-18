using HelixToolkit.Wpf;
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
        private List<BlockType> blockTypes = new List<BlockType>();
        public MainWindow()
        {
            InitializeComponent();
            blockTypes.Add(new BlockType() { BlockCode = 'G', TexturePath = "Resources\\Ground.jpg" });
            blockTypes.Add(new BlockType() { BlockCode = 'W', TexturePath = "Resources\\Wall.jpg" });
            blockTypes.Add(new BlockType() { BlockCode = 'B', TexturePath = "Resources\\Base.jpg" });
            blockTypes.Add(new BlockType() { BlockCode = 'F', TexturePath = "Resources\\Finish.jpg" });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            // Simple block
            Point3D[] blokPunten = { new Point3D(0.5, 0.5, 0.5), new Point3D(-0.5, 0.5, 0.5), new Point3D(-0.5, -0.5, 0.5), new Point3D(0.5, -0.5, 0.5), new Point3D(0.5, 0.5, -0.5), new Point3D(-0.5, 0.5, -0.5), new Point3D(-0.5, -0.5, -0.5), new Point3D(0.5, -0.5, -0.5)};
            //var blok = new Block(blokPunten, @"Resources\Ground.jpg");
            //var blok2 = new Block(blokPunten, @"Resources\Wall.jpg");

            var finBlok = new Block(new Point3D(-1, 0, 0), blockTypes.Where(t => t.BlockCode == 'F').ToList()[0]);
            var baseBlok = new Block(new Point3D(0, 0, 0), blockTypes.Where(t => t.BlockCode == 'B').ToList()[0]);

            // Simple light
            var light = new AmbientLight()
            {
                Color = Colors.White
            };

            // Model elements
            var modelGrp = new Model3DGroup();
            //modelGrp.Children.Add(blok.Model);
            //modelGrp.Children.Add(blok2.Model);
            modelGrp.Children.Add(finBlok.Model);
            modelGrp.Children.Add(baseBlok.Model);
            modelGrp.Children.Add(light);            
            HelixViewport.Children.Add(new ModelVisual3D() { Content = modelGrp });

            // Final linking (?)
            NameScope.SetNameScope(HelixViewport, new NameScope());
            HelixViewport.Camera.Position = new Point3D(0, 0, 10);
            HelixViewport.Camera.LookDirection = new Vector3D(0, 0, -1);
            HelixViewport.Camera.UpDirection = new Vector3D(0, 1, 0);
        }
    }
}
