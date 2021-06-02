using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;

namespace Witness
{
    public class ColorPuzzle : Puzzle<Edge>
    {
        private readonly HashSet<Edge> _edges = new HashSet<Edge>();
        private readonly Rgba32[] _colors = new[] {
            new Rgba32(16, 151, 122), // Puzzle background
            new Rgba32(25, 56, 44), // Black box
            new Rgba32(200, 196, 189), // White box
            new Rgba32(139, 143, 75), // Green box
            new Rgba32(141, 24, 173) // Purple box
        };

        public ColorPuzzle(Region region, Image<Rgba32> image) : base(region, image) { }

        protected override string Description => "Color";

        protected override int GetCellValue(Point pixel)
        {
            var minDistance = int.MaxValue;
            var colorId = -1;
            for (int i = 0; i < _colors.Length; i++)
            {
                var distance = GetDistance(_colors[i], Image[pixel.X, pixel.Y]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    colorId = i;
                }
            }

            return colorId;
        }

        protected override bool ValidateCell(ICell cell) => ValidateColors(cell, new HashSet<ICell>(), new HashSet<int>());

        protected override Edge AddingPathSegment(IVertex from, IVertex to)
        {
            var edge = new Edge(Math.Min(from.X, to.X), Math.Min(from.Y, to.Y), Math.Max(from.X, to.X), Math.Max(to.Y, from.Y));
            _edges.Add(edge);
            return edge;
        }

        protected override void RemovingPathSegment(Edge edge) => _edges.Remove(edge);

        private bool ValidateColors(ICell cell, ISet<ICell> visitedCells, ISet<int> colors)
        {
            if (visitedCells.Contains(cell))
                return true;
            visitedCells.Add(cell);

            if (cell.Value > 0)
                colors.Add(cell.Value);
            // More than one color found in current area, this is not a valid path
            if (colors.Count > 1)
                return false;

            foreach (var neighbor in cell.GetNeighbors())
            {
                var edge = new Edge(Math.Max(cell.X, neighbor.X), Math.Max(cell.Y, neighbor.Y), Math.Min(cell.X, neighbor.X) + 1, Math.Min(cell.Y, neighbor.Y) + 1);
                if (!_edges.Contains(edge))
                    // We're not blocked from this adjacent cell by the proposed path, so validate its color
                    if (!ValidateColors(neighbor, visitedCells, colors))
                        return false;
            }
            return true;
        }

        private static int GetDistance(Rgba32 a, Rgba32 b) => (a.R - b.R) * (a.R - b.R) + (a.G - b.G) * (a.G - b.G) + (a.B - b.B) * (a.B - b.B);
    }
}
