using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public struct Rect3
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }

        public Rect3(Rect3 other)
            : this(other.X, other.Y, other.Z, other.Width, other.Height, other.Depth)
        {
        }

        public Rect3(double width, double height)
            : this(width, height, 0)
        {
        }

        public Rect3(double x, double y, double width, double height)
            : this(x, y, 0, width, height, 0)
        {
        }


        public Rect3(double width, double height, double depth)
            : this(0, 0, 0, width, height, depth)
        {
        }

        public Rect3(Vector3 position, double width, double height, double depth)
            : this(position.X, position.Y, position.Z, width, height, depth)
        {
        }


        public Rect3(double x, double y, double z, double width, double height, double depth)
        {
            X = x;
            Y = y;
            Z = z;
            Width = width;
            Height = height;
            Depth = depth;
        }

        public static Rect3 operator +(Rect3 r, Vector3 v)
        {
            return new Rect3
            {
                X = r.X + v.X,
                Y = r.Y + v.Y,
                Z = r.Z + v.Z,
                Width = r.Width,
                Height = r.Height,
                Depth = r.Depth
            };
        }

        public static Rect3 operator -(Rect3 r, Vector3 v)
        {
            return new Rect3
            {
                X = r.X - v.X,
                Y = r.Y - v.Y,
                Z = r.Z - v.Z,
                Width = r.Width,
                Height = r.Height,
                Depth = r.Depth
            };
        }

        public bool Contains(Vector3 point)
        {
            return (
                point.X >= X && point.X <= X + Width &&
                point.Y >= Y && point.Y <= Y + Height &&
                point.Z >= Z && point.Z <= Z + Depth);
        }

        public bool ContainsXY(Vector3 point)
        {
            return (
                point.X >= X && point.X <= X + Width &&
                point.Y >= Y && point.Y <= Y + Height);
        }

        public bool ContainsYZ(Vector3 point)
        {
            return (
                point.Y >= Y && point.Y <= Y + Height &&
                point.Z >= Z && point.Z <= Z + Depth);
        }

        public bool ContainsXZ(Vector3 point)
        {
            return (
                point.X >= X && point.X <= X + Width &&
                point.Z >= Z && point.Z <= Z + Depth);
        }

        public bool Intersects(Rect3 that)
        {
            Rect3 a = this;
            Rect3 b = that;

            return (
                a.X < b.X + b.Width && a.X + a.Width > b.X &&
                a.Y < b.Y + b.Height && a.Y + a.Height > b.Y &&
                a.Z < b.Z + b.Depth && a.Z + a.Depth > b.Z);
        }

        public bool IntersectsXY(Rect3 that)
        {
            Rect3 a = this;
            Rect3 b = that;

            return (
                a.X < b.X + b.Width && a.X + a.Width > b.X &&
                a.Y < b.Y + b.Height && a.Y + a.Height > b.Y);
        }

        public bool IntersectsYZ(Rect3 that)
        {
            Rect3 a = this;
            Rect3 b = that;

            return (
                a.Y < b.Y + b.Height && a.Y + a.Height > b.Y &&
                a.Z < b.Z + b.Depth && a.Z + a.Depth > b.Z);
        }

        public bool IntersectsXZ(Rect3 that)
        {
            Rect3 a = this;
            Rect3 b = that;

            return (
                a.X < b.X + b.Width && a.X + a.Width > b.X &&
                a.Z < b.Z + b.Depth && a.Z + a.Depth > b.Z);
        }

        public Vector3 Clamp(Vector3 point)
        {
            return new Vector3
            {
                X = Utilities.Clamp(point.X, X, X + Width),
                Y = Utilities.Clamp(point.Y, Y, Y + Height),
                Z = Utilities.Clamp(point.Z, Z, Z + Depth)
            };
        }

        public Vector3 Center()
        {
            return new Vector3(
                X + Width / 2,
                Y + Height / 2,
                Z + Depth / 2);
        }

        public SDL.SDL_Rect ToSdlRect()
        {
            return new SDL.SDL_Rect
            {
                x = (int)X,
                y = (int)Y,
                w = (int)Width,
                h = (int)Height
            };
        }

        public Vector3 GetPosition()
        {
            return new Vector3(X, Y, Z);
        }

        public void SetPosition(Vector3 position)
        {
            X = position.X;
            Y = position.Y;
            Z = position.Z;
        }

        public override string ToString()
        {
            return $"R[ {X:0.000}, {Y:0.000}, {Z:0.000} ] [ {Width:0.000}, {Height:0.000}, {Depth:0.000} ]";
        }
    }
}
