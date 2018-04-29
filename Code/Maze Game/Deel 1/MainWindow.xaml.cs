using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace MazeGame
{
    public partial class MainWindow : Window
    {
        #region Fields
        private OpenFileDialog ofd = new OpenFileDialog() { Filter = "json files (*.json)|*.json", RestoreDirectory = true }; // The OFD for Reading the Maze JSON's
        private Model3DGroup modelGrp = new Model3DGroup(); // The container for each element (cubes and light)
        private MazeData currentMazeData = new MazeData();  // The Maze data that's currently in use
        private List<Block> maze;                           // Container for all blocks in maze
        private Sphere player;                              // Player container
        private bool[] tiltDirection = { false, false, false, false }; // Up,Right,Down,Left
        private Vector3D translationVector;                 // Vector that keeps track of the new player location
        private double tiltXAngle = 0;
        private double tiltYAngle = 0;
        private DispatcherTimer timer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 1000 / Constants.FPS) }; // Animation timer (10ms == 100fps)
        #endregion Fields

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

            // Timer Linking
            timer.Tick += Timer_Tick;

            // Background
            HelixViewport.Background = Constants.BG_COLOR;
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
                var errorMsg = "";
                if (readData.CodeSingle == null) errorMsg = "Error reading the file! (No code)";
                else if (readData.Size == null) errorMsg = "Error reading the file! (No Size)";
                else if (readData.Size.Count != 3) errorMsg = "Error reading the file! (Wrong Size number, 3 expected)";
                else if (readData.Size[0] * readData.Size[1] * readData.Size[2] != readData.CodeSingle.Length) errorMsg = "Error reading the file! (Size and Code do not match in number!) - Size:" + (readData.Size[0] * readData.Size[1] * readData.Size[2]) + " Code:" + readData.CodeSingle.Length;
                if (errorMsg != "")
                {
#if DEBUG
                    Console.WriteLine(errorMsg);
#endif
                    return;
                }

                // Sync current
                currentMazeData = readData;

                // Reset camera
                ResetCamera();

                // AddCubes
                CreateFromCurrentData();

#if DEBUG
                Console.WriteLine("Loaded in new data: " + currentMazeData.Title);
#endif
            }
        }

        /// <summary>
        /// Key Press
        /// </summary>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Update tilts
            switch (e.Key)
            {
                case Key.NumPad8:
                    tiltDirection[0] = true; // up
                    break;
                case Key.NumPad6:
                    tiltDirection[1] = true; // right
                    break;
                case Key.NumPad2:
                    tiltDirection[2] = true; // down
                    break;
                case Key.NumPad4:
                    tiltDirection[3] = true; // left
                    break;
                case Key.NumPad5:
                    ResetCamera(); // midle - Camera reset
                    break;
            }

#if DEBUG
            Console.WriteLine(e.Key + " is down!");
#endif
        }

        /// <summary>
        /// Key Release
        /// </summary>
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            // Update tilts
            switch (e.Key)
            {
                case Key.NumPad8:
                    tiltDirection[0] = false; // up
                    break;
                case Key.NumPad6:
                    tiltDirection[1] = false; // right
                    break;
                case Key.NumPad2:
                    tiltDirection[2] = false; // down
                    break;
                case Key.NumPad4:
                    tiltDirection[3] = false; // left
                    break;
            }

#if DEBUG
            Console.WriteLine(e.Key + " is released!");
#endif
        }
        #endregion Events

        #region Private methods
        /// <summary>
        /// Empties the current model and re-adds the lights
        /// </summary>
        private void ResetModel()
        {
            // Empty everything and add simple light
            modelGrp.Children = new Model3DCollection { new DirectionalLight() { Color = Colors.White } };
        }

        /// <summary>
        /// Resets the camera back to starting position and height defined by CurrentMazData
        /// </summary>
        private void ResetCamera()
        {
            HelixViewport.Camera = new PerspectiveCamera() // OrthoCam is weird and feels unnatural
            {
                Position = new Point3D(0, 0, CalcCameraHeight(45)),
                LookDirection = new Vector3D(0, 0, -1),
                UpDirection = new Vector3D(0, 1, 0)
            };
        }

        /// <summary>
        /// Uses CurrentMazeData to make cubes and the player to add it to the model
        /// </summary>
        private void CreateFromCurrentData()
        {
            // Set Limits, string and counter
            var X = currentMazeData.Size[0];
            var Y = currentMazeData.Size[1];
            var Z = currentMazeData.Size[2];
            var codeString = currentMazeData.CodeSingle;
            var counter = -1;
            maze = new List<Block>();

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

                        // Add block
                        var curr_block = new Block(Constants.EDGE_LENGTH, new Point3D(x, y, z), Constants.BLOCKTYPES.Where(t => t.BlockCode == codeString[counter]).ToList()[0]);
                        maze.Add(curr_block);
                        modelGrp.Children.Add(curr_block.Model);
                    }
                }
            }

            // Add the player aswell
            AddPlayer();
            timer.Start();
        }

        /// <summary>
        /// Creates and adds the player to the model
        /// </summary>
        private void AddPlayer()
        {
            // Remove the current player in the view
            if (player != null) modelGrp.Children.Remove(player.Model);

            // Reset the translation
            translationVector = new Vector3D();

            // Add the new one
            var center = new Point3D(currentMazeData.Spawn[0], currentMazeData.Spawn[1], currentMazeData.Spawn[2]);
            player = new Sphere(center, Constants.RADIUS, Constants.PHI, Constants.THETA, Constants.PLAYER_COLOR);
            modelGrp.Children.Add(player.Model);
        }

        /// <summary>
        /// Aplies a rotateTransform on the board depending on the TiltDir[] and also moves the ball
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Tilt
            var tiltVectorX = new Vector3D(-1, 0, 0);
            var tiltVectorY = new Vector3D(0, 1, 0);
            if (tiltDirection[0]) tiltXAngle += Constants.TILT_INCREMENTS;
            if (tiltDirection[1]) tiltYAngle += Constants.TILT_INCREMENTS;
            if (tiltDirection[2]) tiltXAngle -= Constants.TILT_INCREMENTS;
            if (tiltDirection[3]) tiltYAngle -= Constants.TILT_INCREMENTS;

            // None
            if (!(tiltDirection[0] || tiltDirection[2])) tiltXAngle += (tiltXAngle > 0) ? -Constants.TILT_INCREMENTS : (tiltXAngle < 0) ? Constants.TILT_INCREMENTS:0;
            if (!(tiltDirection[1] || tiltDirection[3])) tiltYAngle += (tiltYAngle > 0) ? -Constants.TILT_INCREMENTS : (tiltYAngle < 0) ? Constants.TILT_INCREMENTS:0;

            // Bounds
            if (tiltXAngle > Constants.TILT_MAX) tiltXAngle = Constants.TILT_MAX;
            if (tiltYAngle > Constants.TILT_MAX) tiltYAngle = Constants.TILT_MAX;
            if (tiltXAngle < -Constants.TILT_MAX) tiltXAngle = -Constants.TILT_MAX;
            if (tiltYAngle < -Constants.TILT_MAX) tiltYAngle = -Constants.TILT_MAX;

            // Apply transform
            var rotX = new RotateTransform3D() { Rotation = new AxisAngleRotation3D(tiltVectorX, tiltXAngle) };
            var rotY = new RotateTransform3D() { Rotation = new AxisAngleRotation3D(tiltVectorY, tiltYAngle) };
            modelGrp.Transform = new Transform3DGroup{ Children = { rotX, rotY } };

            // Move the ball
            double forceX = (tiltXAngle > 0) ? 1 : (tiltXAngle < 0) ? -1 : 0;
            double forceY = (tiltYAngle > 0) ? 1 : (tiltYAngle < 0) ? -1 : 0;
            translationVector.X += (forceY / Constants.FPS) * Constants.PLAYER_SPEED;
            translationVector.Y += (forceX / Constants.FPS) * Constants.PLAYER_SPEED;
            if (player != null) player.Model.Transform = new TranslateTransform3D(translationVector);
        }

        /// <summary>
        /// Calculate the appopriate cameraHeight
        /// </summary>
        /// <param name="fov">The camera field of view in Degrees</param>
        /// <returns></returns>
        private double CalcCameraHeight(double fov)
        {
            if (currentMazeData.Size == null) return 0; // Sanity check
            var longestSide = (currentMazeData.Size[0] > currentMazeData.Size[1]) ? currentMazeData.Size[0] : currentMazeData.Size[1];

            return (Math.Sin(Constants.ToRadians(90 - fov / 2)) * ((longestSide / 2) / (Math.Sin(Constants.ToRadians(fov / 2))))) + currentMazeData.Size[2];
        }
        #endregion Private methods
    }
}
