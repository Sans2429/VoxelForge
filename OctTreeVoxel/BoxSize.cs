using devDept.Geometry;
using System;

namespace OctTreeVoxel
{
    public class BoxSize
    {
        public PointD3 BoxMin;
        public PointD3 BoxMax;
        public BoxSize Clone()
        {
            return new BoxSize { BoxMin = this.BoxMin, BoxMax = this.BoxMax };
        }
    }
    public struct PointD3
    {
        public double X; public double Y; public double Z;
        public PointD3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public PointD3(Point3D point)
        {
            X = point.X;
            Y = point.Y;
            Z = point.Z;
        }

        public static double Distance(PointD3 p1, PointD3 p2)
        {
            return Math.Sqrt
                (
                    Math.Pow(p2.X - p1.X, 2) +
                    Math.Pow(p2.Y - p1.Y, 2) +
                    Math.Pow(p2.Z - p1.Z, 2)
                );
        }
        public static PointD3 Center(PointD3 p1, PointD3 p2)
        {
            return new PointD3(
                    (p1.X + p2.X) / 2.0,
                    (p1.Y + p2.Y) / 2.0,
                    (p1.Z + p2.Z) / 2.0
                );
        }
        public Point3D GetGeometryPoint()
        {
            return new Point3D(X, Y, Z);
        }
        public Vector3D GetVectorFromOrigin()
        {
            return new Vector3D(X, Y, Z);
        }
    }
}
