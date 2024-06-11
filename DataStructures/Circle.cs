using System;
using System.Numerics;

namespace DataStructures
{
    public class Circle : IEquatable<Circle>
    {
        private readonly double x;
        private readonly double y;

        private readonly double radius;

        public Circle(double X, double Y, double radius)
        {
            x = X;
            y = Y;

            this.radius = radius;
        }

        public double Area => Math.PI * radius * radius;

        public double Perimeter => 2 * Math.PI * radius;

        public (double X, double Y) Position => (x, y);

        public double Radius => radius;

        public bool ContainsPoint(Vector2 point)
        {
            var distanceSquared = (point.X - x) * (point.X - x) + (point.Y - y) * (point.Y - y);
            return distanceSquared < radius * radius;
        }

        public bool ContainsPoint((double X, double Y) point)
        {
            var distanceSquared = (point.X - x) * (point.X - x) + (point.Y - y) * (point.Y - y);
            return distanceSquared < radius * radius;
        }

        public bool ContainsPoint((float X, float Y) point)
        {
            var distanceSquared = (point.X - x) * (point.X - x) + (point.Y - y) * (point.Y - y);
            return distanceSquared < radius * radius;
        }

        public bool ContainsPoint((int X, int Y) point)
        {
            var distanceSquared = (point.X - x) * (point.X - x) + (point.Y - y) * (point.Y - y);
            return distanceSquared < radius * radius;
        }
        public bool ContainsPoint(double X, double Y)
        {
            var distanceSquared = (X - x) * (X - x) + (Y - y) * (Y - y);
            return distanceSquared < radius * radius;
        }
        public bool ContainsPoint(float X, float Y)
        {
            var distanceSquared = (X - x) * (X - x) + (Y - y) * (Y - y);
            return distanceSquared < radius * radius;
        }
        public bool ContainsPoint(int X, int Y)
        {
            var distanceSquared = (X - x) * (X - x) + (Y - y) * (Y - y);
            return distanceSquared < radius * radius;
        }
        public bool IsPointOnCircumference(Vector2 point)
        {
            var distanceSquared = (point.X - x) * (point.X - x) + (point.Y - y) * (point.Y - y);
            return distanceSquared == radius * radius;
        }

        public bool IsPointOnCircumference((double X, double Y) point)
        {
            var distanceSquared = (point.X - x) * (point.X - x) + (point.Y - y) * (point.Y - y);
            return distanceSquared == radius * radius;
        }

        public bool IsPointOnCircumference((float X, float Y) point)
        {
            var distanceSquared = (point.X - x) * (point.X - x) + (point.Y - y) * (point.Y - y);
            return distanceSquared == radius * radius;
        }

        public bool IsPointOnCircumference((int X, int Y) point)
        {
            var distanceSquared = (point.X - x) * (point.X - x) + (point.Y - y) * (point.Y - y);
            return distanceSquared == radius * radius;
        }
        public bool IsPointOnCircumference(double X, double Y)
        {
            var distanceSquared = (X - x) * (X - x) + (Y - y) * (Y - y);
            return distanceSquared == radius * radius;
        }
        public bool IsPointOnCircumference(float X, float Y)
        {
            var distanceSquared = (X - x) * (X - x) + (Y - y) * (Y - y);
            return distanceSquared == radius * radius;
        }
        public bool IsPointOnCircumference(int X, int Y)
        {
            var distanceSquared = (X - x) * (X - x) + (Y - y) * (Y - y);
            return distanceSquared == radius * radius;
        }

        bool IEquatable<Circle>.Equals(Circle other)
        {
            return other.x == x && other.y == y && other.radius == radius;
        }
    }
}
