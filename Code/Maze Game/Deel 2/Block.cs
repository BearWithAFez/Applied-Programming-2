using System.Windows;
using System.Windows.Media.Media3D;

namespace MazeGame
{
    public class Block : Shape
    {
        #region Fields
        private Point3D[] simpleCorners; // Front top-right CCW & Back top-right CCW
        public BlockType type;
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Creates a cube with the given corner and type
        /// </summary>
        /// <param name="edge">Size of a single edge</param>
        /// <param name="dbl">Down-Back-Left corner coördinates</param>
        /// <param name="type">Gives the type of cube (used for textures etc.)</param>
        public Block(double edge, Point3D dbl, BlockType type) : base(type.TexturePath)
        {
            // Generate all other points
            Point3D[] simpleCorners = {
                new Point3D(dbl.X + edge, dbl.Y + edge, dbl.Z + edge),
                new Point3D(dbl.X, dbl.Y + edge, dbl.Z + edge),
                new Point3D(dbl.X, dbl.Y + edge, dbl.Z),
                new Point3D(dbl.X + edge, dbl.Y + edge, dbl.Z),
                new Point3D(dbl.X + edge, dbl.Y, dbl.Z + edge),
                new Point3D(dbl.X, dbl.Y, dbl.Z + edge),
                new Point3D(dbl.X, dbl.Y, dbl.Z),
                new Point3D(dbl.X + edge, dbl.Y, dbl.Z)
            };

            // Link
            this.simpleCorners = simpleCorners;
            this.type = type;
            CreateCube();
        }
        #endregion Constructors

        #region Helper Methods
        /// <summary>
        /// Creates a cube with given fields
        /// </summary>
        private void CreateCube()
        {
            // Corners, twice
            for (int i = 0; i < 2; i++) foreach (var point in simpleCorners) corners.Add(point);

            // Indices, grouped per 3 decide wich corners make a triangle            
            foreach (var indice in Constants.INDICES_OUTSIDE) triangles.Add(indice);

            // Textures, Linked to corners as corners
            foreach (var pointPair in Constants.TEXTUREPOINTS) texturePoints.Add(new Point(pointPair[0], pointPair[1]));
        }
        #endregion Helper Methods

        #region Public Methods
        public override bool IsIn(Point3D other)
        {
            // Dimensions check
            if (other.X >= corners[0].X || other.X <= corners[1].X) return false;   // 0 == RVB       1  == LVB
            if (other.Y >= corners[0].Y || other.Y <= corners[4].Y) return false;   // 4 == RAB
            if (other.Z >= corners[0].Z || other.Z <= corners[2].Z) return false;   // 2 == LVO
            return true;
        }

        public override string ToString() => type + " at [" + corners[0] + " - " + corners[6] + "]";

        public bool IsUnder(Point3D point)
        { 
            // Dimensions check
            if (point.X > corners[0].X || point.X < corners[1].X) return false;
            if (point.Y > corners[0].Y || point.Y < corners[4].Y) return false;
            return true;
        }

        public bool IsWall()
        {
            return type.BlockCode == 'W';
        }
        #endregion Public Methods
    }
}
