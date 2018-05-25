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
        private OpenFileDialog ofd = new OpenFileDialog() { Filter = "json files (*.json)|*.json", RestoreDirectory = true };   // The OFD for Reading the Maze JSON's
        private Model3DGroup modelGrp = new Model3DGroup();                                                                     // The container for each element (cubes and light)
        private MazeData currentMazeData = new MazeData();                                                                      // The Maze data that's currently in use
        private List<Block> maze;                                                                                               // Container for all blocks in maze
        private Sphere player;                                                                                                  // Player container
        private bool[] tiltDirection = { false, false, false, false };                                                          // Up,Right,Down,Left
        private Vector3D translationVector;                                                                                     // Vector that keeps track of the new player location
        private (double velocity, double angle, int movDir) axisX = (velocity: 0, angle: 0, movDir: 0);                         // Container for all the X physics
        private (double velocity, double angle, int movDir) axisY = (velocity: 0, angle: 0, movDir: 0);                         // Container for all the Y physics
        private DispatcherTimer timer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 1000 / Constants.FPS) };    // Animation timer (10ms == 100fps)
        private bool gameIsStarted = false;
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
                if (errorMsg != "") return;

                // Sync current
                currentMazeData = readData;

                // Reset camera
                ResetCamera();

                // AddCubes
                CreateFromCurrentData();
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
                case Key.NumPad0:
                    PauseResume(); // Pause the game or continue
                    break;
                case Key.Add:
                    Respawn(); // Respawn the player and reset Physics
                    break;
            }
        }

        /// <summary>
        /// Aplies a rotateTransform on the board depending on the TiltDir[] and also moves the ball
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Tilt the board (user input)
            ApplyTilt();

            // Do the physics 
            PhysicsItteration();

            // Detections
            if (PlayerIsDead())
            {
                player.UpdateMaterial(Constants.DEAD);
                HaltMovement();
            }
            if (PlayerIsFinished())
            {
                player.UpdateMaterial(Constants.FINISH);
                HaltMovement();
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
            double X = currentMazeData.Size[0];
            double Y = currentMazeData.Size[1];
            double Z = currentMazeData.Size[2];
            var codeString = currentMazeData.CodeSingle;
            var counter = -1;
            maze = new List<Block>();

            // Loop over all Layers
            for (double z = -Z / 2; z < Z / 2; z++)
            {
                // Loop over all Rows
                for (double y = Y / 2; y > -Y / 2; y--)
                {
                    // Loop over all Cubes
                    for (double x = -X / 2; x < X / 2; x++)
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

            // Add player and add physics
            Respawn();

            // Enable the game
            timer.Start();
            gameIsStarted = true;
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
            player = new Sphere(center, Constants.RADIUS, Constants.PHI, Constants.THETA, Constants.ALIVE);
            modelGrp.Children.Add(player.Model);
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

        private Block PlayerCollision()
        {
            foreach (var cube in maze) if (cube.type.isWall) if (cube.IsIn(player)) return cube;
            return null;
        }

        private bool PlayerIsDead()
        {
            return GetBlockUnderPlayer() == null;
        }

        private bool PlayerIsFinished()
        {
            return player.IsIn(new Point3D(currentMazeData.Goal[0], currentMazeData.Goal[1], currentMazeData.Goal[2]));
        }

        private Block GetBlockUnderPlayer()
        {
            foreach (var cube in maze) if (cube.IsUnder(player.CurrentCenter)) return cube;
            return null;
        }

        private void PauseResume()
        {
            if (!gameIsStarted) return;
            if (timer.IsEnabled) timer.Stop();
            else timer.Start();
        }

        private void Respawn()
        {
            // Reset the physics
            axisX = (velocity: 0, angle: 0, movDir: 0);
            axisY = (velocity: 0, angle: 0, movDir: 0);

            // Re-add the player
            AddPlayer();
        }

        private void ApplyTilt()
        {
            // Tilt
            if (tiltDirection[0]) axisX.angle += Constants.TILT_INCREMENTS;
            if (tiltDirection[1]) axisY.angle += Constants.TILT_INCREMENTS;
            if (tiltDirection[2]) axisX.angle -= Constants.TILT_INCREMENTS;
            if (tiltDirection[3]) axisY.angle -= Constants.TILT_INCREMENTS;

            // None
            if (!(tiltDirection[0] || tiltDirection[2])) axisX.angle += (axisX.angle > 0) ? -Constants.TILT_INCREMENTS : (axisX.angle < 0) ? Constants.TILT_INCREMENTS : 0;
            if (!(tiltDirection[1] || tiltDirection[3])) axisY.angle += (axisY.angle > 0) ? -Constants.TILT_INCREMENTS : (axisY.angle < 0) ? Constants.TILT_INCREMENTS : 0;

            // Bounds
            if (axisX.angle > Constants.TILT_MAX) axisX.angle = Constants.TILT_MAX;
            if (axisY.angle > Constants.TILT_MAX) axisY.angle = Constants.TILT_MAX;
            if (axisX.angle < -Constants.TILT_MAX) axisX.angle = -Constants.TILT_MAX;
            if (axisY.angle < -Constants.TILT_MAX) axisY.angle = -Constants.TILT_MAX;

            // Apply transform
            var rotX = new RotateTransform3D() { Rotation = new AxisAngleRotation3D(new Vector3D(-1, 0, 0), axisX.angle) };
            var rotY = new RotateTransform3D() { Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), axisY.angle) };
            modelGrp.Transform = new Transform3DGroup { Children = { rotX, rotY } };
        }

        private void PhysicsItteration()
        {
            // Get the block underneath the player
            var bop = GetBlockUnderPlayer();
            var bopt = (bop == null) ? Constants.BLOCKTYPES[0] : bop.type;

            // Endstate?
            if (PlayerIsDead() || PlayerIsFinished()) return;

            // Get the whole physics calculation
            var (displacementX, displacementY, velocityX, velocityY, moveDirX, moveDirY) = Physics.PhysicsLoop(bopt, axisY.angle, axisX.angle, Constants.BALL_WEIGHT, axisX.movDir, axisY.movDir, axisX.velocity, axisY.velocity);

            // Try moving the Y
            var prev_Y = translationVector.Y;   // Save previous position
            translationVector.Y += displacementY;   // Try to move
            if (player != null) player.Model.Transform = new TranslateTransform3D(translationVector);
            var cube = PlayerCollision();
            if (cube != null)   // Collision!
            {
                translationVector.Y = prev_Y;   // Reset
                var cur_displacementY = .0;
                while (cube.IsIn(player))   // Move step by step
                {
                    if (displacementY == 0) break;  // No movement
                    if (Math.Abs(cur_displacementY) > Math.Abs(displacementY)) break;
                    translationVector.Y += displacementY / Constants.BOUNCE_STEPS;
                    cur_displacementY += displacementY / Constants.BOUNCE_STEPS;
                    if (player != null) player.Model.Transform = new TranslateTransform3D(translationVector);
                }
                // Do bounce
                moveDirY *= -1;
                velocityY *= -cube.type.k;
                translationVector.Y -= displacementY;
                if (player != null) player.Model.Transform = new TranslateTransform3D(translationVector);
            }

            // Try moving the X
            var prev_X = translationVector.X;   // Save previous position
            translationVector.X += displacementX;   // Try to move
            if (player != null) player.Model.Transform = new TranslateTransform3D(translationVector);
            cube = PlayerCollision();
            if (cube != null)   // Collision!
            {
                translationVector.X = prev_X;   // Reset
                var cur_displacementX = .0;
                while (cube.IsIn(player))   // Move step by step
                {
                    if (displacementX == 0) break;  // No movement
                    if (Math.Abs(cur_displacementX) > Math.Abs(displacementX)) break;
                    translationVector.X += displacementX / Constants.BOUNCE_STEPS;
                    cur_displacementX += displacementX / Constants.BOUNCE_STEPS;
                    if (player != null) player.Model.Transform = new TranslateTransform3D(translationVector);
                }
                // Do bounce
                moveDirX *= -1;
                velocityX *= -cube.type.k;
                translationVector.X -= displacementX;
                if (player != null) player.Model.Transform = new TranslateTransform3D(translationVector);
            }

            // Update the current physics
            axisX.movDir = moveDirX;
            axisX.velocity = velocityX;
            axisY.movDir = moveDirY;
            axisY.velocity = velocityY;
        }

        private void HaltMovement()
        {
            // Halt movement in X
            axisX.movDir = 0;
            axisX.velocity = 0;

            // Halt movement in Y
            axisY.movDir = 0;
            axisY.velocity = 0;
        }
        #endregion Private methods
    }
}
