using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    // TODO: Consider whether this should be a struct
    // https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct
    public struct Vector3
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public static readonly Vector3 Up = new Vector3(0, -1, 0);
        public static readonly Vector3 Down = new Vector3(0, 1, 0);
        public static readonly Vector3 Left = new Vector3(-1, 0, 0);
        public static readonly Vector3 Right = new Vector3(1, 0, 0);
        public static readonly Vector3 Forward = new Vector3(0, 0, 1);
        public static readonly Vector3 Back = new Vector3(0, 0, -1);
        public static readonly Vector3 One = new Vector3(1, 1, 1);
        public static readonly Vector3 Zero = new Vector3(0, 0, 0);

        public double Magnitude
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }

        public Vector3 Normalized
        {
            get
            {
                return this / Magnitude;
            }
        }

        public Vector3(double x, double y)
            : this(x, y, 0)
        {
        }

        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3(Vector3 other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }

        public static Vector3 FromPolar(double radius, double thetaRadians)
        {
            return new Vector3
            {
                X = radius * Math.Cos(thetaRadians),
                Y = radius * Math.Sin(thetaRadians)
            };
        }

        public static Vector3 operator +(Vector3 a) => a;
        public static Vector3 operator -(Vector3 a)
        {
            return new Vector3
            {
                X = -a.X,
                Y = -a.Y,
                Z = -a.Z
            };
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3
            {
                X = a.X + b.X,
                Y = a.Y + b.Y,
                Z = a.Z + b.Z
            };
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3
            {
                X = a.X - b.X,
                Y = a.Y - b.Y,
                Z = a.Z - b.Z
            };
        }

        public static Vector3 operator *(Vector3 a, double c)
        {
            return new Vector3
            {
                X = c * a.X,
                Y = c * a.Y,
                Z = c * a.Z
            };
        }

        public static Vector3 operator /(Vector3 a, double c)
        {
            return new Vector3
            {
                X = a.X / c,
                Y = a.Y / c,
                Z = a.Z / c
            };
        }

        public SDL.SDL_Point ToSdlPoint()
        {
            return new SDL.SDL_Point
            {
                x = (int)X,
                y = (int)Y
            };
        }

        public override string ToString()
        {
            return $"V[ {X:0.000}, {Y:0.000}, {Z:0.000} ]";
        }
    }
}
