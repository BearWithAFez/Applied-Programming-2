using System.Windows;
using System.Windows.Media.Media3D;

namespace MazeGame
{
    public class Block : Shape
    {
        #region Fields
        private Point3D[] simpleCorners; // Front top-right CCW & Back top-right CCW
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
            CreateCube();
#if DEBUG
            System.Console.WriteLine("Cube added @ " + new Point3D(dbl.X + .5, dbl.Y + .5, dbl.Z + .5));
#endif
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
    }
}
