using SixLabors.ImageSharp;
using System;

namespace Witness
{
    public class Edge
    {
        public Edge(int minX, int minY, int maxX, int maxY)
        {
            Point1 = new Point(minX, minY);
            Point2 = new Point(maxX, maxY);
        }

        public Point Point1 { get; }
        public Point Point2 { get; }

        public override bool Equals(object obj) => obj is Edge edge && Point1.Equals(edge.Point1) && Point2.Equals(edge.Point2);

        public override int GetHashCode() => HashCode.Combine(Point1, Point2);
    }
}
