using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PixelPortal
{
    public class PortalHandler
    {
        private Game1 game;
        private Random random = new Random();
        private Color blueColor = new Color(70, 85, 255);
        private Color yellowColor = new Color(255, 240, 95);
        public Projectile blueProjectile;
        public Projectile yellowProjectile;
        public Portal portalB;
        public Portal portalY;
        
        public PortalHandler(Game1 game)
        {
            this.game = game;
        }
        public void SetPortal(Point tile, Point indir, bool flipped, bool isBlue)
        {
            Portal[] parr = [portalB, portalY];
            foreach (Portal portal in parr)
            {
                if (portal == null) continue;
                if (portal.tile == tile && portal.inDirection == indir) return;
            }
            
            if (isBlue)
            {
                if (portalB != null) { DestroyPortal(portalB); }
                portalB = new Portal(tile, indir, flipped);
            }
            else
            {
                if (portalY != null) { DestroyPortal(portalY); }
                portalY = new Portal(tile, indir, flipped);
            }
        }
        public void DestroyPortal(Portal portalToDestroy)
        {
            portalToDestroy = null;
        }
        public Portal GetLinkedPortal(Portal p)
        {
            if(p == portalB) { return portalY; }
            if(p == portalY) {  return portalB; }
            return null;
        }
        public void RemoveProjectile(bool isBlue)
        {
            Projectile proj = isBlue ? blueProjectile : yellowProjectile;
            game.physicsWorld.RemoveEntity(proj);
            game.visuals.Remove(proj as IDrawable);
            if(isBlue ) {blueProjectile = null;}
            else {yellowProjectile = null;}
        }
        public void SpawnProjectile(bool isBlue, Vector2 spawnPoint, Vector2 direction)
        {
            direction.Normalize();

            if (isBlue && blueProjectile != null)
            {
                RemoveProjectile(isBlue);
            }
            else if (!isBlue && yellowProjectile != null)
            {
                RemoveProjectile(isBlue);
            }
            
            Projectile proj = new Projectile(
                this, 
                isBlue,
                direction,
                game.tilemap, 
                animatedSprite: isBlue ? game.blueProjectileAnim : game.yellowProjectileAnim, 
                position: spawnPoint, 
                rotation: MathF.Atan2(direction.Y, direction.X), 
                spriteEffects: direction.X < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None
            );
            proj.colorMultiplier = isBlue ? blueColor : yellowColor;
            proj.flipped = direction.X < 0 ? true : false;

            game.physicsWorld.AddEntity(proj);
            game.visuals.Add(proj as IDrawable);
            float soundPitch = ((float)random.NextDouble() - 0.5f) * 0.4f;
            float soundDuration = (float)random.NextDouble() + 0.3f;
            if (isBlue) {
                blueProjectile = proj;
                SoundHandler.PlaySoundEffectDecay(0, pitch: soundPitch - 0.2f, duration: soundDuration);
            }
            else { 
                yellowProjectile = proj;
                SoundHandler.PlaySoundEffectDecay(0, pitch: soundPitch + 0.2f, duration: soundDuration);
            }
            
        }
        public bool TileHasDisabledCollision(Point tile, Point inDirection) 
        {
            Portal[] parr = [portalB, portalY];
            foreach ( Portal portal in parr)
            {
                if (portal == null) continue;
                if (portal.tile == tile && portal.inDirection == inDirection && GetLinkedPortal(portal) != null) return true;
            }
            return false; 
        } 
        public Portal GetPortalFromTile(Point tile, Point inDir)
        {
            Portal[] parr = [portalB, portalY];
            foreach (Portal portal in parr)
            {
                if (portal == null) continue;
                if (portal.tile == tile && portal.inDirection == inDir) return portal;
            }
            return null;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            int tileSize = Tilemap.TileSize;
            Portal[] toDraw = [portalB, portalY ];
            foreach (Portal portal in toDraw)
            {
                if (portal == null) continue;
                Point origin = Tilemap.TileTOPosCenterP(portal.tile);

                Color col = portal == portalB ? blueColor : yellowColor; //fråga anders om bättre sätt, indexerad for loop? inkludera färg i portal klassen?

                spriteBatch.Draw(
                   game.bluePortalFrameTexture,
                   new Rectangle(origin.X, origin.Y, tileSize, tileSize),
                   null,
                   col,
                   -MathF.Atan2(portal.inDirection.X, portal.inDirection.Y),
                   new Vector2(4, 4),
                   portal.flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                   0
               );
            }

        }

    }
}
