using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Collections.Generic;
using System.Linq;

namespace Witness
{
    public class PuzzleIllustrator
    {
        private readonly IPuzzle _puzzle;

        public PuzzleIllustrator(IPuzzle puzzle)
        {
            _puzzle = puzzle;
        }

        public Image<Rgba32> Illustrate(IEnumerable<IVertex> path)
        {
            var image = _puzzle.Image.Clone();
            DrawBoundingBox(image);
            DrawCellCenters(image);
            DrawPath(image, path);
            return image;
        }

        private void DrawBoundingBox(Image<Rgba32> image) =>
            image.Mutate(i => i.Draw(Color.Red, 2, _puzzle.BoundingBox));

        private void DrawCellCenters(Image<Rgba32> image)
        {
            var pen = Pens.Solid(Color.Red, 2);
            foreach (var c in _puzzle.Cells)
                image.Mutate(i =>
                    i.DrawLines(pen, new Point(c.Location.X - 10, c.Location.Y), new Point(c.Location.X + 10, c.Location.Y))
                     .DrawLines(pen, new Point(c.Location.X, c.Location.Y - 10), new Point(c.Location.X, c.Location.Y + 10)));
        }

        private void DrawPath(Image<Rgba32> image, IEnumerable<IVertex> path)
        {
            var pen = Pens.Solid(Color.White, 6);
            if (path == null)
            {
                var bb = _puzzle.BoundingBox;
                image.Mutate(i =>
                    i.DrawLines(pen, new Point(bb.Left, bb.Bottom), new Point(bb.Right, bb.Top))
                     .DrawLines(pen, new Point(bb.Left, bb.Top), new Point(bb.Right, bb.Bottom)));
                return;
            }
            image.Mutate(i => i.DrawLines(pen, path.Select(v => (PointF)v.Location).ToArray()));
        }
    }
}
