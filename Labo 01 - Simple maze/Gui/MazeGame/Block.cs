using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace MazeGame
{
    public class Block
    {
        // Helpers
        private static Int32[] INDICES = { 0, 1, 2, 0, 2, 3, 4, 7, 6, 4, 6, 5, 4, 0, 3, 4, 3, 7, 1, 5, 6, 1, 6, 2, 9, 8, 12, 9, 12, 13, 10, 14, 15, 10, 15, 11 };
        
        // Fields
        public GeometryModel3D Model = new GeometryModel3D(); 
        public MeshGeometry3D Mesh = new MeshGeometry3D();
        public Point3DCollection Corners = new Point3DCollection();

        // Constructors
        public Block(Point3D[] points, string textureUrl)
        {
            // Preparation
            Int32Collection triangles = new Int32Collection();
            PointCollection texturePoints = new PointCollection();

            // Linking
            Mesh.Positions = Corners;
            Mesh.TriangleIndices = triangles;
            Mesh.TextureCoordinates = texturePoints;
            Model.Geometry = Mesh;
            Model.Material = new DiffuseMaterial(new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(textureUrl, UriKind.Relative))
            });

            // Corners, twice (TextureFix)
            for(int i = 0; i < 2; i++) foreach (var point in points) Corners.Add(point);

            // Indices, grouped per 3 decide wich corners make a triangle            
            foreach (var indice in INDICES) triangles.Add(indice);

            // Textures, Same as corners
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
    }
}
