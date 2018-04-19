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
            // Model elements
            var modelGrp = new Model3DGroup();
            HelixViewport.Children.Add(new ModelVisual3D() { Content = modelGrp });

            // Simple light
            modelGrp.Children.Add(new AmbientLight() { Color = Colors.White });

            /*
            // Blocks
            var res = AddCubesFromString(new Point3D(2, 2, 2), "BEFEEGEW", modelGrp);
            */

            // Simple blocks
            var finBlok = new Block(new Point3D(-1, 0, 0), blockTypes.Where(t => t.BlockCode == 'F').ToList()[0]);
            modelGrp.Children.Add(finBlok.Model);
            var baseBlok = new Block(new Point3D(0, 0, 0), blockTypes.Where(t => t.BlockCode == 'B').ToList()[0]);
            modelGrp.Children.Add(baseBlok.Model);
            var wallBlok = new Block(new Point3D(-1, -1, 0), blockTypes.Where(t => t.BlockCode == 'W').ToList()[0]);
            modelGrp.Children.Add(wallBlok.Model);
            var grondBlok = new Block(new Point3D(0, -1, 0), blockTypes.Where(t => t.BlockCode == 'G').ToList()[0]);
            modelGrp.Children.Add(grondBlok.Model);

            // Final linking (?)
            NameScope.SetNameScope(HelixViewport, new NameScope());
            HelixViewport.Camera.Position = new Point3D(0, 0, 10);
            HelixViewport.Camera.LookDirection = new Vector3D(0, 0, -1);
            HelixViewport.Camera.UpDirection = new Vector3D(0, 1, 0);
        }
        
        /*
        private bool AddCubesFromString(Point3D dimensions, string codeString, Model3DGroup mdlGrp)
        {
            // Wrong dimensions
            if (dimensions.X * dimensions.Y * dimensions.Z != codeString.Length) return false;

            // Counter
            var counter = 0;

            // Loop over all Layers
            for (var z = -dimensions.Z / 2; z < dimensions.Z / 2; z++)
            {
                // Loop over all Rows
                for (var y = dimensions.Y / 2; y > -dimensions.Y / 2; y--)
                {
                    // Loop over all Cubes
                    for (var x = -dimensions.X / 2; x < dimensions.X / 2; x++)
                    {
                        if (codeString[counter] == 'E') continue; // Empty block
                        mdlGrp.Children.Add(new Block(new Point3D(x, y, z), blockTypes.Where(t => t.BlockCode == codeString[counter]).ToList()[0]).Model);
                    }
                }
            }
            return true;
        }
        */
    }
}
