using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace MazeGame
{
    public struct Constants
    {
        // General
        public static int FPS = 60;
        public static double TILT_INCREMENTS = 4;
        public static double TILT_MAX = 20;
        public static Brush BG_COLOR = Brushes.Gray; // I like gray

        // Block
        public static double EDGE_LENGTH = 1;   // Free to choose, 1 default
        public static int[][] TEXTUREPOINTS =   // Texture coordinates
            { new int[]{ 1, 0 }, new int[]{ 0, 0 }, new int[]{ 0, 1 }, new int[]{ 1, 1 }, new int[]{ 0, 0 }, new int[]{ 1, 0 }, new int[]{ 1, 1 }, new int[]{ 0, 1 }, new int[]{ 1, 1 }, new int[]{ 0, 1 }, new int[]{ 0, 0 }, new int[]{ 1, 0 }, new int[]{ 1, 0 }, new int[]{ 0, 0 }, new int[]{ 0, 1 }, new int[]{ 1, 1 } };
        public static Int32[] INDICES_OUTSIDE = // Collection of all indices used to make a cube.
            { 0, 2, 1, 0, 3, 2, 4, 6, 7, 4, 5, 6, 4, 3, 0, 4, 7, 3, 1, 6, 5, 1, 2, 6, 9, 12, 8, 9, 13, 12, 10, 15, 14, 10, 11, 15 };
        public static Int32[] INDICES_INSIDE =  // Collection of all indices used to make a cube.
            { 0, 1, 2, 0, 2, 3, 4, 7, 6, 4, 6, 5, 4, 0, 3, 4, 3, 7, 1, 5, 6, 1, 6, 2, 9, 8, 12, 9, 12, 13, 10, 14, 15, 10, 15, 11 };
        public static List<BlockType> BLOCKTYPES = new BlockType[]  // All types of possible blocks
        {
            new BlockType() { BlockCode = 'G', TexturePath = "Resources\\Ground.jpg", µs = .2, µk = .1, k = 1, isWall = false },
            new BlockType() { BlockCode = 'B', TexturePath = "Resources\\Base.jpg", µs = .2, µk = .1, k = 1, isWall = false },
            new BlockType() { BlockCode = 'F', TexturePath = "Resources\\Finish.jpg", µs = .2, µk = .1, k = 1, isWall = false },
            new BlockType() { BlockCode = 'I', TexturePath = "Resources\\Ice.jpg", µs = .001, µk = .001, k = 1, isWall = false },
            new BlockType() { BlockCode = 'S', TexturePath = "Resources\\Sand.jpg", µs = .3, µk = .3, k = 1, isWall = false },
            new BlockType() { BlockCode = 'W', TexturePath = "Resources\\Wall.jpg", µs = 1, µk = 1, k = .6, isWall = true },
            new BlockType() { BlockCode = 'J', TexturePath = "Resources\\Jello.jpg", µs = 1, µk = 1, k = 1.3, isWall = true },
            new BlockType() { BlockCode = 'P', TexturePath = "Resources\\Pillow.jpg", µs = 1, µk = 1, k = .01, isWall = true }
        }.ToList(); 

        // Sphere
        public static double RADIUS = .5;               // Best half of cube edge
        public static int PHI = 20;                     // Best between 10-1000
        public static int THETA = 20;                   // Best between 10-1000
        public static Color ALIVE = Colors.Orange;      // I like orange
        public static Color DEAD = Colors.IndianRed;    // I like this red
        public static Color FINISH = Colors.LightGreen; // I like smooth greens


        // Physics
        public static double G = 1;  // In nature its 9.81 m/s/s
        public static double MAX_V = 10000;
        public static double DISPLACEMENT_SCALING = .0001;
        public static double BALL_WEIGHT = 1;
        public static int BOUNCE_STEPS = 10;

        // Methods
        public static double ToRadians(double angle) => Math.PI * angle / 180.0;    
    }

    public struct MazeData
    {
        public string Title;
        public List<int> Size;
        public string CodeSingle;
        public List<double> Spawn;
        public List<double> Goal;
    }

    public struct BlockType
    {
        public string TexturePath;
        public char BlockCode;
        public double µs;   // Static friction
        public double µk;   // Kinetic friction
        public double k;    // Bounce coeficient
        public bool isWall;
        public override string ToString() => "" + BlockCode;
    }
}
