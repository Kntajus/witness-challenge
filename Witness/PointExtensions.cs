using SixLabors.ImageSharp;

namespace Witness
{
    public static class PointExtensions
    {
        public static Point[] GetNeighbors(this Point p) =>
            new[] { new Point(p.X + 1, p.Y), new Point(p.X, p.Y - 1), new Point(p.X - 1, p.Y), new Point(p.X, p.Y + 1) };
    }
}
