using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace PixelPortal
{
    public class Portal
    {
        private PortalHandler handler;
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
