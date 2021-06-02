using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Witness
{
    public class Region
    {
        private readonly List<ConnectedPoint> _points = new List<ConnectedPoint>();

        private Point _topLeft = new Point(int.MaxValue, int.MaxValue);
        private Point _bottomRight = new Point(int.MinValue, int.MinValue);
        private Rectangle? _boundingBox;

        public void Add(ConnectedPoint point)
        {
            _boundingBox = null;
            _points.Add(point);
            _topLeft.X = Math.Min(_topLeft.X, point.X);
            _topLeft.Y = Math.Min(_topLeft.Y, point.Y);
            _bottomRight.X = Math.Max(_bottomRight.X, point.X);
            _bottomRight.Y = Math.Max(_bottomRight.Y, point.Y);
        }

        public int Intensity => _points.Count > 10 ? _points.Sum(p => p.Intensity) / _points.Count : 255;
        public int Count => _points.Count;
        public int Width => _bottomRight.X - _topLeft.X;
        public int Height => _bottomRight.Y - _topLeft.Y;
        public double CoverageRatio => (double)_points.Count / Width / Height;
        public double AspectRatio => (double)Width / Height;

        public double BoundingAspectRatio
        {
            get
            {
                EnsureBoundingBox();
                return _boundingBox.Value.Height != 0 ? (double)_boundingBox.Value.Width / _boundingBox.Value.Height : 0.0;
            }
        }

        public Rectangle BoundingBox
        {
            get
            {
                EnsureBoundingBox();
                return _boundingBox.Value;
            }
        }

        private Point Center => new Point((_topLeft.X + _bottomRight.X) / 2, (_topLeft.Y + _bottomRight.Y) / 2);

        private void EnsureBoundingBox()
        {
            if (_boundingBox.HasValue)
                return;

            var top = int.MaxValue;
            var left = int.MaxValue;
            var bottom = int.MinValue;
            var right = int.MinValue;
            // Essentially, this is looking for points in a + shape, outwards from the center point of the region
            // The region we're "looking for" is a 4x4 puzzle grid, in which case this should be forming a cross
            // right through the center of the puzzle, along its pathways; we can then use that to define a
            // "bounding box" around the key part of the puzzle that we care about.
            foreach (var p in _points)
            {
                if (p.Y == Center.Y)
                {
                    left = Math.Min(left, p.X);
                    right = Math.Max(right, p.X);
                }
                if (p.X == Center.X)
                {
                    top = Math.Min(top, p.Y);
                    bottom = Math.Max(bottom, p.Y);
                }
            }
            _boundingBox = new Rectangle(left, top, right - left, bottom - top);
        }
    }
}
