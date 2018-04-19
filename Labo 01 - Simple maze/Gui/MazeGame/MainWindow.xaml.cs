using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MazeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<BlockType> blockTypes = new List<BlockType>();
        private OpenFileDialog ofd = new OpenFileDialog() { Filter = "json files (*.json)|*.json", RestoreDirectory = true };
        private Model3DGroup modelGrp = new Model3DGroup();
        private MazeData currentMazeData = new MazeData();

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
            // reset field
            ResetModel();

            // Model elements
            HelixViewport.Children.Add(new ModelVisual3D() { Content = modelGrp });

            // Blocks
            var res = AddCubesFromString(new Point3D(24, 28, 2), "GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGFFGGGGGGGGGGGGGGGGGGGGGGFFGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGEEGGGGGGGGEEGGGGGGEEGGGGEEGGGGGGGGEEGGGGGGEEGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGEEGGGGGGGGGGGGGGGEEGGGGGEEGGGGGGGGGGGGGGGEEGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGEEGGGGGGGGGGGGGGGGGGGGGGEEGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGBBGGGGGGGGEEGGGGGGGGGGGGBBGGGGGGGGEEGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGEEGGGGGGGGGGGGGGGGEEGGGGEEGGGGGGGGGGGGGGGGEEGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGWWWWWWWWWWWWWWWWWWWWWWWWWEEEEEEEEEEEEEEWEEEEEEEWWEEEEEEEEEEEEEEWEEEEEEEWWEEEEEEEEEEEEEEWEEEEEEEWWEEEEEEEEEEEEEEEEEEEEEEWWEEEEEEEEEEEEEEEEEEEEEEWWEEEEEEEEEWEEEEEEEEEEEEWWEEEEEEEEEWEEEEEEEEEEEEWWEEEEEEEEEWEEEEEEEEEEEEWWWWWEEEWWWWWWWWWWWWWWWWWWEEEEEEEEEEEEEEEEEEEEEEWWEEEEEEEEEEEEEEEEEEEEEEWWEEEEEEEEEEEEEEEEEEEEEEWWEEEEEEEEEEEEEEEEEEEEEEWWEEEEEEEEEEEEEEEEEEEEEEWWEEEEEEEEEEEEEEEEEEEEEEWWEEEEEEEEEEEEEEEEEEEEEEWWEEEEEEEEEEEEEEEEEEEEEEWWWWWWWWWWWWWWWWWWWEEEWWWWEEEEEWEEEEEEEEEEEEEEEEWWEEEEEWEEEEEEEEEEEEEEEEWWEEEEEWEEEEEEEEEEEEEEEEWWEEEEEEEEEEEEEEEEEEEEEEWWEEEEEEEEEEEEEEEEEEEEEEWWEEEEEEEEWEEEEEEWEEEEEEWWEEEEEEEEWEEEEEEWEEEEEEWWEEEEEEEEWEEEEEEWEEEEEEWWWWWWWWWWWWWWWWWWWWWWWWW");

            // Final linking (?)
            NameScope.SetNameScope(HelixViewport, new NameScope());
            HelixViewport.Camera.Position = new Point3D(0, 0, 40);
            HelixViewport.Camera.LookDirection = new Vector3D(0, 0, -1);
            HelixViewport.Camera.UpDirection = new Vector3D(0, 1, 0);
        }

        private void ResetModel()
        {
            // Empty everything
            modelGrp.Children = new Model3DCollection();

            // Simple light
            modelGrp.Children.Add(new AmbientLight() { Color = Colors.White });
        }
        
        private bool AddCubesFromString(Point3D dimensions, string codeString)
        {
            // Wrong dimensions
            if (dimensions.X * dimensions.Y * dimensions.Z != codeString.Length) return false;

            // Counter
            var counter = -1;

            // Loop over all Layers
            for (var z = -dimensions.Z / 2; z < dimensions.Z / 2; z++)
            {
                // Loop over all Rows
                for (var y = dimensions.Y / 2; y > -dimensions.Y / 2; y--)
                {
                    // Loop over all Cubes
                    for (var x = -dimensions.X / 2; x < dimensions.X / 2; x++)
                    {
                        counter++;
                        if (codeString[counter] == 'E') continue; // Empty block
                        modelGrp.Children.Add(new Block(new Point3D(x, y, z), blockTypes.Where(t => t.BlockCode == codeString[counter]).ToList()[0]).Model);
                    }
                }
            }
            return true;
        }

        private bool MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            // File loading
            if (ofd.ShowDialog() == true)
            {
                // Reset the Model
                ResetModel();

                // Read data
                Stream myStream;
                MazeData data = new MazeData();
                try
                {
                    if ((myStream = ofd.OpenFile()) != null) using (StreamReader r = new StreamReader(myStream)) data = JsonConvert.DeserializeObject<MazeData>(r.ReadToEnd());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error reading the file!");
                }
            }
        }
    }

    public struct MazeData
    {
        public string Title;
        public List<int> Size;
        public List<string> CodeCompact;
        public List<string> CodeBig;
        public string CodeSingle;
    }
}
