using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;

namespace Witness
{
    public class RegionFinder
    {
        private readonly Image<Rgba32> _image;
        private readonly bool[,] _pixelVisited;
        private Point _pixel;

        public RegionFinder(Image<Rgba32> image)
        {
            _image = image.Clone(i => i.Grayscale());
            _pixelVisited = new bool[image.Width, image.Height];
        }

        public Region GetNext() => SetStartPoint() ? GetConnectedPixels() : null;

        private bool SetStartPoint()
        {
            while (_pixel.X < _image.Width)
            {
                while (_pixel.Y < _image.Height)
                {
                    if (!_pixelVisited[_pixel.X, _pixel.Y])
                        return true;
                    _pixel.Y++;
                }
                _pixel.X++;
                _pixel.Y = 0;
            }
            return false;
        }

        private Region GetConnectedPixels()
        {
            var region = new Region();
            var stack = new Stack<ConnectedPoint>();
            stack.Push(CreateConnectedPoint(_pixel));

            while (stack.Count > 0)
            {
                var pixel = stack.Pop();
                if (_pixelVisited[pixel.X, pixel.Y] || Math.Abs(pixel.Intensity - pixel.ParentIntensity) > 1)
                    continue;
                _pixelVisited[pixel.X, pixel.Y] = true;
                region.Add(pixel);

                foreach (var neighbor in ((Point)pixel).GetNeighbors())
                {
                    if (neighbor.X < 0 || neighbor.X == _image.Width || neighbor.Y < 0 || neighbor.Y == _image.Height || _pixelVisited[neighbor.X, neighbor.Y])
                        continue;
                    stack.Push(CreateConnectedPoint(neighbor, pixel.Intensity));
                }
            }
            return region;
        }

        private ConnectedPoint CreateConnectedPoint(Point point, byte? parentIntensity = null)
        {
            var intensity = _image[point.X, point.Y].B;
            return new ConnectedPoint(point, intensity, parentIntensity ?? intensity);
        }
    }
}
