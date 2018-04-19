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
    public partial class MainWindow : Window
    {
        #region Private fields
        private OpenFileDialog ofd = new OpenFileDialog() { Filter = "json files (*.json)|*.json", RestoreDirectory = true }; // The OFD for Reading the Maze JSON's
        private Model3DGroup modelGrp = new Model3DGroup(); // The container for each element (cubes and light)
        private MazeData currentMazeData = new MazeData(); // The Maze data that's currently in use
        private List<BlockType> blockTypes = new BlockType[] 
        {
            new BlockType() { BlockCode = 'G', TexturePath = "Resources\\Ground.jpg" },
            new BlockType() { BlockCode = 'W', TexturePath = "Resources\\Wall.jpg" },
            new BlockType() { BlockCode = 'B', TexturePath = "Resources\\Base.jpg" },
            new BlockType() { BlockCode = 'F', TexturePath = "Resources\\Finish.jpg" }
        }.ToList(); // All types of possible blocks
        #endregion Private fields

        #region Constructors
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion Constructors

        #region Events
        /// <summary>
        /// Basic on Load pre-setter
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // reset field
            ResetModel();

            // Model elements
            HelixViewport.Children.Add(new ModelVisual3D() { Content = modelGrp });

            // Camera setup
            ResetCamera();

            // Final linking
            NameScope.SetNameScope(HelixViewport, new NameScope()); // needed for animations etc
        }

        /// <summary>
        /// Load data
        /// </summary>
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
                if (readData.CameraHeight == 0) // No camera Height
                {
                    Console.WriteLine("Error reading the file! (No CameraHeight)");
                    return;
                }
                if (readData.CodeSingle == null) // No CodeSingle
                {
                    Console.WriteLine("Error reading the file! (No code)");
                    return;
                }
                if (readData.Size == null) // No dimensions
                {
                    Console.WriteLine("Error reading the file! (No Size)");
                    return;
                }
                if (readData.Size.Count != 3) // Wrong # of dimensions
                {
                    Console.WriteLine("Error reading the file! (Wrong Size number, 3 expected)");
                    return;
                }
                if (readData.Size[0] * readData.Size[1] * readData.Size[2] != readData.CodeSingle.Length) // Dimensions and String not compatible
                {
                    Console.WriteLine("Error reading the file! (Size and Code do not match in number!) - Size:" + (readData.Size[0] * readData.Size[1] * readData.Size[2]) + " Code:" + readData.CodeSingle.Length);
                    return;
                }

                // Sync current
                currentMazeData = readData;

                // Set camera
                ResetCamera();

                // AddCubes
                AddCubesFromCurrentData();

                Console.WriteLine("Loaded in new data: " + currentMazeData.Title);
            }
        }
        #endregion Events

        #region Private methods
        /// <summary>
        /// Empties the current model and re-adds the lights
        /// </summary>
        private void ResetModel()
        {
            // Empty everything and add simple light
            modelGrp.Children = new Model3DCollection { new AmbientLight() { Color = Colors.White } };
        }

        /// <summary>
        /// Resets the camera back to starting position and height defined by CurrentMazData
        /// </summary>
        private void ResetCamera()
        {
            HelixViewport.Camera = new PerspectiveCamera() // OrthoCam is weird and feels unnatural
            {
                Position = new Point3D(0, 0, currentMazeData.CameraHeight),
                LookDirection = new Vector3D(0, 0, -1),
                UpDirection = new Vector3D(0, 1, 0)
            };
        }

        /// <summary>
        /// Uses CurrentMazeData to make cubes and adds it to the model
        /// </summary>
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
        #endregion Private methods
    }
}
