using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MazeGame
{
    public class Sphere : Shape
    {
        #region Private Fields
        private static double radius = 0.4;
        private int phi;
        private int theta;
        private Point3D center;
        private Dictionary<Point3D, int> points = new Dictionary<Point3D, int>();
        #endregion Private Fields

        #region Fields
        public Point3D Center {
            get { return center; }
            set { center = value; CreateSphere(); }
        }
        public int Phi {
            get { return phi; }
            set { phi = value; CreateSphere(); }
        }
        public int Theta {
            get { return theta; }
            set { theta = value; CreateSphere(); }
        }
        #endregion Fields

        public Sphere(Point3D center, int phi, int theta ,Color color) : base("")
        {
            Model.Material = new DiffuseMaterial(new SolidColorBrush(color));
            this.center = center;
            this.phi = phi;
            this.theta = theta;
            CreateSphere();
        }

        private void CreateSphere()
        {
            // Constants?
            double dphi = Math.PI / phi;
            double dtheta = 2 * Math.PI / theta;

            double phi0 = 0;
            double y0 = radius * Math.Cos(phi0);
            double r0 = radius * Math.Sin(phi0);
            for (int i = 0; i < phi; i++)
            {
                double phi1 = phi0 + dphi;
                double y1 = radius * Math.Cos(phi1);
                double r1 = radius * Math.Sin(phi1);

                // Point ptAB has phi value A and theta value B.
                // For example, pt01 has phi = phi0 and theta = theta1.
                // Find the points with theta = theta0.
                double theta0 = 0;
                Point3D pt00 = new Point3D(
                    center.X + r0 * Math.Cos(theta0),
                    center.Y + y0,
                    center.Z + r0 * Math.Sin(theta0));
                Point3D pt10 = new Point3D(
                    center.X + r1 * Math.Cos(theta0),
                    center.Y + y1,
                    center.Z + r1 * Math.Sin(theta0));

                for (int j = 0; j < theta; j++)
                {
                    // Find the points with theta = theta1.
                    double theta1 = theta0 + dtheta;
                    Point3D pt01 = new Point3D(
                        center.X + r0 * Math.Cos(theta1),
                        center.Y + y0,
                        center.Z + r0 * Math.Sin(theta1));
                    Point3D pt11 = new Point3D(
                        center.X + r1 * Math.Cos(theta1),
                        center.Y + y1,
                        center.Z + r1 * Math.Sin(theta1));

                    // Create the triangles.
                    AddTriangle(pt00, pt11, pt10);
                    AddTriangle(pt00, pt01, pt11);

                    // Move to the next value of theta.
                    theta0 = theta1;
                    pt00 = pt01;
                    pt10 = pt11;
                }

                // Move to the next value of phi.
                phi0 = phi1;
                y0 = y1;
                r0 = r1;
            }
        }

        // Add a triangle to the indicated mesh.
        // Reuse points so triangles share normals.
        private void AddTriangle(Point3D p0, Point3D p1, Point3D p2)
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

            // If two or more of the points are
            // the same, it's not a triangle.
            if ((index1 == index2) || (index2 == index3) || (index3 == index1)) return;

            // Create the triangle.
            mesh.TriangleIndices.Add(index1);
            mesh.TriangleIndices.Add(index2);
            mesh.TriangleIndices.Add(index3);
        }
    }
}
