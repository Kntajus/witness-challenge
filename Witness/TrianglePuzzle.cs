using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;

namespace Witness
{
    public class TrianglePuzzle : Puzzle<IEnumerable<ICell>>
    {
        private readonly Random _random = new Random();
        private readonly int[,] _cellWalls;

        public TrianglePuzzle(Region region, Image<Rgba32> image) : base(region, image)
        {
            _cellWalls = new int[GridSize, GridSize];
        }

        protected override string Description => "Triangle";

        protected override int GetCellValue(Point pixel)
        {
            var samples = 1000;
            var maxDeltaX = CellSize / 2;
            var maxDeltaY = maxDeltaX / 3; // The triangles are only displayed across the middle ~third of the cell
            var intensity = 0;
            for (int i = 0; i < samples; i++)
            {
                var sampleX = GetSampleValue(pixel.X, maxDeltaX);
                var sampleY = GetSampleValue(pixel.Y, maxDeltaY);
                intensity += GrayImage[sampleX, sampleY].B;
            }
            intensity /= samples;

            return intensity < 23 ? 0 : intensity < 63 ? 1 : intensity < 91 ? 2 : 3;
        }

        protected override bool ValidateCell(ICell cell) => cell.Value == 0 || cell.Value == _cellWalls[cell.X, cell.Y];

        protected override IEnumerable<ICell> AddingPathSegment(IVertex from, IVertex to)
        {
            var adjacentCells = new List<ICell>();
            // These are the 2 (potential - one might be off the board) cells that the proposed move touches, and hence increases the count for
            if (TryGetCell(Math.Max(from.X, to.X) - 1, Math.Max(from.Y, to.Y) - 1, out var cell))
                adjacentCells.Add(cell);
            if (TryGetCell(Math.Min(from.X, to.X), Math.Min(from.Y, to.Y), out cell))
                adjacentCells.Add(cell);

            // If we're already at the wall limit for any adjacent cells, the proposed move would be invalid, so abandon this path
            foreach (var adjacent in adjacentCells)
                if (adjacent.Value > 0 && _cellWalls[adjacent.X, adjacent.Y] == adjacent.Value)
                    return null;

            foreach (var adjacent in adjacentCells)
                _cellWalls[adjacent.X, adjacent.Y]++;

            return adjacentCells;
        }

        protected override void RemovingPathSegment(IEnumerable<ICell> adjacentCells)
        {
            foreach (var adjacent in adjacentCells)
                _cellWalls[adjacent.X, adjacent.Y]--;
        }

        private int GetSampleValue(int center, int maxDelta) => center - maxDelta + _random.Next(2 * maxDelta + 1);
    }
}
