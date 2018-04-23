using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace MazeGame
{
    public class Block : Shape
    {
        #region Helpers
        private static double RIB = 1;
        private static Int32[] INDICES = { 0, 2, 1, 0, 3, 2, 4, 6, 7, 4, 5, 6, 4, 3, 0, 4, 7, 3, 1, 6, 5, 1, 2, 6, 9, 12, 8, 9, 13, 12, 10, 15, 14, 10, 11, 15 };   // outsides
        //  { 0, 1, 2, 0, 2, 3, 4, 7, 6, 4, 6, 5, 4, 0, 3, 4, 3, 7, 1, 5, 6, 1, 6, 2, 9, 8, 12, 9, 12, 13, 10, 14, 15, 10, 15, 11 };                                // insides
        #endregion Helpers

        #region Constructors
        public Block(Point3D UFL, BlockType type) : base(type.TexturePath)
        {
            // Generate all other points
            Point3D[] points = {
                new Point3D(UFL.X + RIB, UFL.Y + RIB, UFL.Z + RIB),
                new Point3D(UFL.X, UFL.Y + RIB, UFL.Z + RIB),
                new Point3D(UFL.X, UFL.Y + RIB, UFL.Z),
                new Point3D(UFL.X + RIB, UFL.Y + RIB, UFL.Z),
                new Point3D(UFL.X + RIB, UFL.Y, UFL.Z + RIB),
                new Point3D(UFL.X, UFL.Y, UFL.Z + RIB),
                new Point3D(UFL.X, UFL.Y, UFL.Z),
                new Point3D(UFL.X + RIB, UFL.Y, UFL.Z)
            };

            CreateCube(points);
        }

        public Block(Point3D[] points, string textureUrl) : base(textureUrl)
        {
            CreateCube(points);
        }
        #endregion Constructors

        #region Helper Methods
        private void CreateCube(Point3D[] points)
        {
            // Corners, twice (TextureFix)
            for (int i = 0; i < 2; i++) foreach (var point in points) corners.Add(point);

            // Indices, grouped per 3 decide wich corners make a triangle            
            foreach (var indice in INDICES) triangles.Add(indice);

            // Textures, Linked to corners as corners
            texturePoints.Add(new Point(1, 0));
            texturePoints.Add(new Point(0, 0));
            texturePoints.Add(new Point(0, 1));
            texturePoints.Add(new Point(1, 1));

            texturePoints.Add(new Point(0, 0));
            texturePoints.Add(new Point(1, 0));
            texturePoints.Add(new Point(1, 1));
            texturePoints.Add(new Point(0, 1));

            texturePoints.Add(new Point(1, 1));
            texturePoints.Add(new Point(0, 1));
            texturePoints.Add(new Point(0, 0));
            texturePoints.Add(new Point(1, 0));

            texturePoints.Add(new Point(1, 0));
            texturePoints.Add(new Point(0, 0));
            texturePoints.Add(new Point(0, 1));
            texturePoints.Add(new Point(1, 1));
        }
        #endregion Helper Methods
    }
}
