using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace MazeGame
{
    public abstract class Shape
    {
        #region Private Fields
        protected MeshGeometry3D mesh;
        protected Point3DCollection corners;
        protected Int32Collection triangles;
        protected PointCollection texturePoints;
        #endregion Private Fields

        #region Fields
        public GeometryModel3D Model;
        #endregion Fields

        #region Constructor
        protected Shape(string textureUrl)
        {
            // Preparation
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
    }
}