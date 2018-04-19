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

            // Final linking (?)
            // Todo: explain this
            NameScope.SetNameScope(HelixViewport, new NameScope());
            HelixViewport.Camera.Position = new Point3D(0, 0, 40);
            HelixViewport.Camera.LookDirection = new Vector3D(0, 0, -1);
            HelixViewport.Camera.UpDirection = new Vector3D(0, 1, 0);
        }

        private void ResetModel()
        {
            // Empty everything and add simple light
            modelGrp.Children = new Model3DCollection { new AmbientLight() { Color = Colors.White } };
        }

        private void AddCubesFromCurrentData()
        {
            // Set Limits, string and counter
            var X = currentMazeData.Size[0];
            var Y = currentMazeData.Size[1];
            var Z = currentMazeData.Size[2];
            var codeString = currentMazeData.CodeSingle;
            var counter = -1;

            // Loop over all Layers
            for (var z = -Z / 2; z < Z / 2; z++)
            {
                // Loop over all Rows
                for (var y = Y / 2; y > -Y / 2; y--)
                {
                    // Loop over all Cubes
                    for (var x = -X / 2; x < X / 2; x++)
                    {
                        counter++;
                        if (codeString[counter] == 'E') continue; // Empty block
                        modelGrp.Children.Add(new Block(new Point3D(x, y, z), blockTypes.Where(t => t.BlockCode == codeString[counter]).ToList()[0]).Model);
                    }
                }
            }
        }

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            // File loading
            if (ofd.ShowDialog() == true)
            {
                // Reset the Model
                ResetModel();

                // Read data
                Stream myStream;
                MazeData readData = new MazeData();
                try
                {
                    if ((myStream = ofd.OpenFile()) != null) using (StreamReader r = new StreamReader(myStream)) readData = JsonConvert.DeserializeObject<MazeData>(r.ReadToEnd());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error reading the file! MSG>" + ex.Message);
                    return;
                }
                // Check data
                // Todo: Say why it failed
                if (readData.CodeSingle == null) return; // No CodeSingle
                if (readData.Size == null) return; // No dimensions
                if (readData.Size.Count != 3) return; // Wrong # of dimensions
                if (readData.Size[0] * readData.Size[1] * readData.Size[2] != readData.CodeSingle.Length) return; // Dimensions and String not compatible

                // Sync current
                currentMazeData = readData;

                // AddCubes
                AddCubesFromCurrentData();

                Console.WriteLine("Loaded in new data: " + currentMazeData.Title);
            }
        }
    }

    public struct MazeData
    {
        public string Title;
        public List<int> Size;
        public string CodeSingle;
    }
}
