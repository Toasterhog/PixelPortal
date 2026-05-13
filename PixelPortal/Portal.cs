using Microsoft.Xna.Framework;

namespace PixelPortal
{
    public class Portal
    {
        public readonly Point tile;
        public readonly Point inDirection;
        public readonly bool flipped;

        public Portal(Point tile, Point indir, bool flipped)
        {
            this.tile = tile;
            this.flipped = flipped;
            this.inDirection = indir;
        }
    }
}
