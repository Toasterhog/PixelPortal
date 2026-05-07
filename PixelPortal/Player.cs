using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace PixelPortal
{
    public class Player : PhysicalEntity
    {

        const int SPEED = 200;
        float defaultBouncinas;
        public Player(PortalHandler portalSystem, Tilemap tilemap, float collisionradious = 10) : base( portalSystem, tilemap, collisionradious)
        {
            defaultBouncinas = bounciness;
        }
        public Player(PortalHandler portalSystem, Tilemap tilemap, Texture2D texture = null, AnimatedSprite animatedSprite = null, float collisionradious = 10, Vector2? position = null, float rotation = 0, float scale = 1, SpriteEffects spriteEffects = SpriteEffects.None, float layerDepth = 0)
        : base(portalSystem, tilemap, texture, animatedSprite, collisionradious, position, rotation, scale, spriteEffects, layerDepth)
        {
            defaultBouncinas = bounciness;
        }

        public override void PhysicsUpdate(float delta)
        {
            MovementInput();
            base.PhysicsUpdate(delta);
        }

        public void MovementInput()
        {
            Input i = Game1.input;
            int animseq = -1;
            int spriteflipp = 0;
            AdvancedSprite a = animatedSprite as AdvancedSprite;
            //if (velocity.X > 100000) { spriteflipp = 1; }
            //else if (velocity.X < 100000) { spriteflipp = -1; }
            if (velocity.LengthSquared() > 400000) { animseq = 2; }
            //else if (a.Sequence == 2 && velocity.LengthSquared() < 5) { animseq = 0; }

            if (i.IsKeyDown(Keys.D) && velocity.X < SPEED)
            {
                velocity.X += 30;
                animseq = 3;
            }
            if (i.IsKeyDown(Keys.A) && velocity.X > -SPEED)
            {
                velocity.X -= 30;
                animseq = 3;
            }
            if (i.IsKeyJustPressed(Keys.Space))
            {
                bounciness = 1;
                if (CollidingGroundward()) velocity.Y -= 200;

            }
            if (i.IsKeyJustReleased(Keys.Space))
            {
                bounciness = defaultBouncinas;
            }

            

            if (i.IsKeyJustReleased(Keys.D)) { animseq = 0; spriteflipp = 0; }
            if (i.IsKeyJustReleased(Keys.A)) { animseq = 0; spriteflipp = 0; }
            if (i.IsKeyJustPressed(Keys.D)) { animseq = 3; spriteflipp = 1; }
            if (i.IsKeyJustPressed(Keys.A)) { animseq = 3; spriteflipp = -1; }
            

            if (animseq != -1 && animseq != a.Sequence) { a.Sequence = animseq; }
            if (spriteflipp != 0)
            {
                spriteEffects = spriteflipp < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            }
            

        }

        bool CollidingGroundward()
        {
            float margin = 2; //2pixlar över marken räknas som kollision här
            Point tpos = Tilemap.PosToTile(position);
            Point collTile = tpos + new Point(0,1);
            if (tilemap.GetTileType(collTile) >= 0)
            {
                if (portalSys.TileHasDisabledCollision(tpos, new Point(0, 1)))
                {
                    return false;
                }
                Vector2 tileOffsetVector = Vector2.UnitY;
                Vector2 boundry = tileOffsetVector * tileSize * 0.5f;
                Vector2 localPos = Mathlike.WrapV(position, new Vector2(tileSize, tileSize)) - new Vector2(tileSize / 2f, tileSize / 2f);
                Vector2 entityBoundry = localPos + tileOffsetVector * collisionradious;
                //float overlap = Mathlike.ProjectionFactor(entityBoundry, boundry) - 1;
                float overlap = Vector2.Dot(entityBoundry, boundry) / (tileSize / 2) - (tileSize / 2);
                if (overlap > -margin)
                {
                    return true;
                }
            }
            return false;
        }

        protected override void CollisionReactionAxisAligned(Point tile, Vector2 normal, float overlap)
        {
            base.CollisionReactionAxisAligned(tile, normal, overlap);
            AdvancedSprite a = animatedSprite as AdvancedSprite;
            if (a.Sequence == 2 ) { a.Sequence = 0; }
        }

    }
}
