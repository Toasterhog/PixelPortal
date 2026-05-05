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
            int animseq = 0;
            if(i.IsKeyDown(Keys.D) && velocity.X < SPEED)
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
            AdvancedSprite a = animatedSprite as AdvancedSprite; 
            a.SetSequence(animseq);
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
    }
}
