using DataStructures;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace PoissonDiskSampling
{
    public static class PoissonDiskSampling
    {
        public static IEnumerable<Circle> Apply(IEnumerable<Vector2> points, double radius = 1) {
            return ApplyInternal(points.Select(p => ((double)p.X, (double)p.Y)), radius);
        }

        public static IEnumerable<Circle> Apply(IEnumerable<(float X, float Y)> points, double radius = 1)
        {
            return ApplyInternal(points.Select(p => ((double)p.X, (double)p.Y)), radius);
        }

        public static IEnumerable<Circle> Apply(IEnumerable<(double X, double Y)> points, double radius = 1)
        {
            return ApplyInternal(points, radius);
        }
        public static IEnumerable<Circle> Apply(IEnumerable<(int X, int Y)> points, double radius = 1)
        {
            return ApplyInternal(points.Select(p => ((double)p.X, (double)p.Y)), radius);
        }


        private static IEnumerable<Circle> ApplyInternal(IEnumerable<(double X, double Y)> points, double radius = 1)
        {
            var poissonDisks = new HashSet<Circle>();

            var pointsRemaining = new HashSet<(double X, double Y)>(points);

            while (pointsRemaining.Count > 0)
            {
                var disk = new Circle(pointsRemaining.First().X, pointsRemaining.First().Y, radius);
                var pointsInDisk = new HashSet<(double X, double Y)>(points.Count());
                foreach(var point in pointsRemaining)
                {
                    if(disk.ContainsPoint(point))
                        pointsInDisk.Add(point);
                }

                poissonDisks.Add(disk);

                pointsRemaining = pointsRemaining.Except(pointsInDisk).ToHashSet();
            }
            return poissonDisks;
        }
    }
}
