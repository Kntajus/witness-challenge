using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Witness
{
    public class PuzzleFinder
    {
        public IPuzzle GetPuzzle(string imagePath)
        {
            var image = Image.Load<Rgba32>(imagePath);
            // Assume the screenshot is wider than it is high. We're expecting a square puzzle in the centre
            // of the screenshot, so trim off the sides to reduce the area we have to search.
            image.Mutate(i => i.Crop(new Rectangle((image.Width - image.Height) / 2, 0, image.Height, image.Height)));

            var finder = new RegionFinder(image);
            var region = finder.GetNext();
            while (region != null)
            {
                if (IsPuzzle(region))
                    break;
                region = finder.GetNext();
            }

            if (region == null)
                return null;
            if (region.Intensity < 60)
                return new ColorPuzzle(region, image);
            return new TrianglePuzzle(region, image);
        }

        private static bool IsPuzzle(Region region)
        {
            // Heuristic calculated based on the various checks detailed below
            // The region we're looking for should be made up of the pathways in the puzzle
            return
                region.Intensity > 30 && // Average brightness high enough
                region.Count > 100 && // Enough connected pixels
                region.Width > 150 && region.Height > 150 && // Large enough
                region.CoverageRatio > 0.35 && region.CoverageRatio < 0.5 && // Draw a bounding box around the region. How much of the area is covered by the region?
                region.AspectRatio > 0.8 && region.AspectRatio < 1.25 && // Width/Height - ideally it's a square but the screenshot may be at a bit of an angle
                region.BoundingAspectRatio > 0.8 && region.BoundingAspectRatio < 1.25; // Width/Height of the points reachable from the center (should be along center pathways)
        }
    }
}
