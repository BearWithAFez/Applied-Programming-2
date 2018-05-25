using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MazeGame
{
    public class Sphere : Shape
    {
        #region Fields
        private double radius;
        private int phi;        // The amount of "parts" the shape consists of top to bottom
        private int theta;      // The amount of "parts" the shape consists of horizontaly
        private Point3D center;
        #endregion Fields

        #region Properties
        public Point3D CurrentCenter {
            get{
                var trans = (Model.Transform as TranslateTransform3D);
                if (trans == null) return center;
                return Point3D.Add(center, new Vector3D(trans.OffsetX, trans.OffsetY, 0));
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a sphere with the given parameters
        /// </summary>
        /// <param name="center">Coördinates of the center</param>
        /// <param name="radius">The radius</param>
        /// <param name="phi">Amount of Vertical "slices"(Best between 10 - 1000)</param>
        /// <param name="theta">Amount of Horizontal "slices"(Best between 10 - 1000)</param>
        /// <param name="color">The color</param>
        public Sphere(Point3D center, double radius, int phi, int theta, Color color) : base("")
        {
            this.center = center;
            this.phi = phi;
            this.theta = theta;
            this.radius = radius;
            Model.Material = new DiffuseMaterial(new SolidColorBrush(color));
            CreateSphere();
        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Creates a sphere based on the given values
        /// </summary>
        private void CreateSphere()
        {
            // Calculate the deltas between points in RADs
            double d_phi = Math.PI / phi;
            double d_theta = 2 * Math.PI / theta;

            // Vertical looping (top to bot)
            for (double curr_phi = 0; curr_phi < d_phi * phi; curr_phi += d_phi)
            {
                // Pre-calc current vals
                double curr_Z = radius * Math.Cos(curr_phi);
                double curr_radius = radius * Math.Sin(curr_phi);

                // Pre-calc next vals
                double next_phi = curr_phi + d_phi;
                double next_Z = radius * Math.Cos(next_phi);
                double next_radius = radius * Math.Sin(next_phi);

                // Horizontal looping (left to right)
                for (double curr_theta = 0; curr_theta < d_theta * theta; curr_theta += d_theta)
                {
                    // Pre-calc next theta
                    double next_theta = curr_theta + d_theta;

                    // Calculate all 4 points
                    var pt00 = new Point3D()
                    {
                        X = center.X + curr_radius * Math.Cos(curr_theta),
                        Y = center.Y + curr_radius * Math.Sin(curr_theta),
                        Z = center.Z + curr_Z
                    };
                    var pt10 = new Point3D()
                    {
                        X = center.X + next_radius * Math.Cos(curr_theta),
                        Y = center.Y + next_radius * Math.Sin(curr_theta),
                        Z = center.Z + next_Z
                    };
                    var pt01 = new Point3D()
                    {
                        X = center.X + curr_radius * Math.Cos(next_theta),
                        Y = center.Y + curr_radius * Math.Sin(next_theta),
                        Z = center.Z + curr_Z
                    };
                    var pt11 = new Point3D
                    {
                        X = center.X + next_radius * Math.Cos(next_theta),
                        Y = center.Y + next_radius * Math.Sin(next_theta),
                        Z = center.Z + next_Z
                    };

                    // Create the triangles
                    AddTriangle(pt00, pt11, pt10);
                    AddTriangle(pt00, pt01, pt11);
                }
            }
        }

        public override bool IsIn(Point3D other)
        {
            // Transforms
            var trans = (Model.Transform as TranslateTransform3D);
            var x = (trans == null) ? 0 : trans.OffsetX;
            var y = (trans == null) ? 0 : trans.OffsetY;
            var z = (trans == null) ? 0 : trans.OffsetZ;

            // See if distance between points is smaller than radius
            return (Point3D.Add(center, new Vector3D(x, y, z)) - other).Length < radius;
        }

        public void UpdateMaterial(Color newColor)
        {
            Model.Material = new DiffuseMaterial(new SolidColorBrush(newColor));
        }
        #endregion Methods
    }
}