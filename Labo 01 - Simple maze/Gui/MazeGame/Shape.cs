using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace MazeGame
{
    public abstract class Shape
    { 
        #region Fields
        /// <summary>
        /// Object that holds the corners, indices and texturepoints
        /// </summary>
        protected MeshGeometry3D mesh;
        /// <summary>
        /// Collection that holds all coördinates that make the current shape
        /// </summary>
        protected Point3DCollection corners;
        /// <summary>
        /// Collection that holds all indices, triangles, based on the corners, (0,1,2) means the first to third corner
        /// </summary>
        protected Int32Collection triangles;
        /// <summary>
        /// Collection that describes per CORNER what texture coördinate corresponds with it
        /// </summary>
        protected PointCollection texturePoints;

        private Dictionary<Point3D, int> points;
        #endregion Fields

        #region Properties
        /// <summary>
        /// The "MAIN" object that represents the entire shape
        /// </summary>
        public GeometryModel3D Model;
        #endregion Properties

        #region Constructor
        /// <summary>
        /// Simple creation and linking of everything
        /// </summary>
        /// <param name="textureUrl">The URL to the used texture, leave blank if none</param>
        protected Shape(string textureUrl)
        {
            // Simple creation and inner linking
            points = new Dictionary<Point3D, int>();
            corners = new Point3DCollection();
            triangles = new Int32Collection();
            texturePoints = new PointCollection();
            mesh = new MeshGeometry3D()
            {
                Positions = corners,
                TriangleIndices = triangles,
                TextureCoordinates = texturePoints
            };
            Model = new GeometryModel3D()
            {
                Geometry = mesh,
                Material = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(textureUrl, UriKind.Relative))))
            };
        }
        #endregion Constructor

        #region Methods
        /// <summary>
        /// Method that takes 3 points, add them as corners if not duplicate, and makes an indice of them
        /// </summary>
        /// <param name="p0">The first point</param>
        /// <param name="p1">The second point</param>
        /// <param name="p2">The third point</param>
        protected void AddTriangle(Point3D p0, Point3D p1, Point3D p2)
        {
            int index1, index2, index3;

            // Find or create the points.
            if (points.ContainsKey(p0)) index1 = points[p0];
            else
            {
                index1 = corners.Count;
                corners.Add(p0);
                points.Add(p0, index1);
            }

            if (points.ContainsKey(p1)) index2 = points[p1];
            else
            {
                index2 = corners.Count;
                corners.Add(p1);
                points.Add(p1, index2);
            }

            if (points.ContainsKey(p2)) index3 = points[p2];
            else
            {
                index3 = corners.Count;
                corners.Add(p2);
                points.Add(p2, index3);
            }

            // Sanity check
            if ((index1 == index2) || (index2 == index3) || (index3 == index1)) return;

            // Create the triangle.
            triangles.Add(index1);
            triangles.Add(index2);
            triangles.Add(index3);
        }
        #endregion Methods
    }
}