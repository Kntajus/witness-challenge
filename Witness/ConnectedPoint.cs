using SixLabors.ImageSharp;

namespace Witness
{
    public struct ConnectedPoint
    {
        private readonly Point _point;

        public ConnectedPoint(Point point, byte intensity, byte parentIntensity)
        {
            _point = point;
            Intensity = intensity;
            ParentIntensity = parentIntensity;
        }

        public int X => _point.X;
        public int Y => _point.Y;
        public byte Intensity { get; }
        public byte ParentIntensity { get; }

        public static explicit operator Point(ConnectedPoint cp) => cp._point;
    }
}
