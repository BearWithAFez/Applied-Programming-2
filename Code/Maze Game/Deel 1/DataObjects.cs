using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace MazeGame
{
    public struct Constants
    {
        // General
        public static int FPS = 100;
        public static double ToRadians(double angle) => Math.PI * angle / 180.0;
        public static double PLAYER_SPEED = 3;
        public static double TILT_INCREMENTS = 1;
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
            new BlockType() { BlockCode = 'G', TexturePath = "Resources\\Ground.jpg" },
            new BlockType() { BlockCode = 'W', TexturePath = "Resources\\Wall.jpg" },
            new BlockType() { BlockCode = 'B', TexturePath = "Resources\\Base.jpg" },
            new BlockType() { BlockCode = 'F', TexturePath = "Resources\\Finish.jpg" }
        }.ToList(); 

        // Sphere
        public static double RADIUS = .5;       // Best half of cube edge
        public static int PHI = 20;             // Best between 10-1000
        public static int THETA = 20;           // Best between 10-1000
        public static Color PLAYER_COLOR =      // I like orange
            Colors.Orange;
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
    }
}
