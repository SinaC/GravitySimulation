using System;

namespace Math
{
    public class Vector3
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3(Vector3 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X+v2.X, v1.Y+v2.Y, v1.Z+v2.Z);
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector3 operator *(Vector3 v, double d)
        {
            return new Vector3(v.X*d, v.Y*d, v.Z*d);
        }

        public static Vector3 operator /(Vector3 v, double d)
        {
            return new Vector3(v.X / d, v.Y / d, v.Z / d);
        }

        public static Vector3 Normalize(Vector3 v)
        {
            Vector3 newV = new Vector3(v);
            newV.Normalize();
            return newV;
        }

        public double LengthSquared()
        {
            return X*X + Y*Y + Z*Z;
        }

        public double Length()
        {
            return System.Math.Sqrt(LengthSquared());
        }

        public void Normalize()
        {
            double length = Length();
            X /= length;
            Y /= length;
            Z /= length;
        }

        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}]", X, Y, Z);
        }
    }
}
