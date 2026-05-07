using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;


namespace PixelPortal
{
    public class Projectile : PhysicalEntity 
    {
        private PortalHandler portalHandler;
        private bool isBlue;
        const int COLLRAD = 5;
        const float SPEED = 900;
        private bool continueSteppingPU = true;

        public Projectile(PortalHandler portal, bool typeIsBlue, Vector2 direction, Tilemap tilemap, Texture2D texture = null, AnimatedSprite animatedSprite = null, float collisionradious = 10, Vector2? position = null, float rotation = 0, float scale = 1, SpriteEffects spriteEffects = SpriteEffects.None, float layerDepth = 0)
        : base(portal, tilemap, texture, animatedSprite, COLLRAD, position, rotation, 3, spriteEffects, layerDepth)
        {
            isBlue = typeIsBlue;
            this.portalHandler = portal;
            gravity = Vector2.Zero;
            velocity = direction * SPEED;
            origin = new Vector2(6f, 3.5f);
            continueSteppingPU = true;
        }


        public override void PhysicsUpdate(float delta)
        {
            int steps = (int)MathF.Ceiling(SPEED * delta / collisionradious);
            float steppedDelta = delta / steps;
            for (int i = 0; i < steps && continueSteppingPU; i++)
            {
                base.PhysicsUpdate(steppedDelta);
            }
        }


        public void RemoveSelf()
        {
            portalHandler.RemoveProjectile(isBlue);
            continueSteppingPU = false;
        }

        
        protected override void CollisionReactionAxisAligned(Point tile, Vector2 normal, float overlap)
        {
            bool flipp = Mathlike.TwoDCrossProduct(normal, velocity) > 0;
            Point collidedTile = tile - normal.ToPoint();
            if (tilemap.GetTileType(collidedTile) == 1) //white
            {
                portalHandler.SetPortal(tile, (-normal).ToPoint(), flipp, isBlue);
            }
            RemoveSelf();
        }
        protected override void CollisionReactionDiagonal(Point tile, Vector2 normal, float overlap)
        {
            RemoveSelf();
        }

        protected override void ClampVelocity(float delta) //bc stepped delta
        {
            //float ls = velocity.LengthSquared();
            //if ( ls > collisionradious)
            //{
            //    velocity = Vector2.Normalize(velocity) * collisionradious / delta;
            //}
            
        }

        protected override void AfterTeleportLogic() {
            rotation = MathF.Atan2(velocity.Y, velocity.X);
        }

        //public override void PhysicsUpdate(GameTime gameTime)
        //{
        //    float delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds * simulationSpeed / 1000;
        //    //const float tileSize = 50;

        //    Point oldTileC = Tilemap.PosToTile(position);

        //    //velocity += gravity * delta;
        //    //velocity = new Vector2(MathF.Min(MathF.Max(velocity.X, -collisionradious / delta), collisionradious / delta), MathF.Min(MathF.Max(velocity.Y, -collisionradious / delta), collisionradious / delta));
        //    position += velocity * delta;

        //    Point newTileC = Tilemap.PosToTile(position);

        //    if (newTileC == oldTileC) return;
        //    if(tilemap.GetTileType(newTileC) >= 0)
        //    {
        //        HitReaction(oldTileC, newTileC);
        //    }
        //}

        //private void HitReaction(Point tile, Point collidedTile)
        //{

        //    Point diff = tile - collidedTile;
        //    bool tilesAreAxisAligned = diff.X == 0 || diff.Y == 0 ? true : false;

        //    if (tilesAreAxisAligned && tilemap.GetTileType(collidedTile) == 1) //white
        //    {
        //        portalHandler.SetPortal(tile, collidedTile - tile, isLeft, isBlue);
        //    }
        //    RemoveSelf();
        //}
    }
}
