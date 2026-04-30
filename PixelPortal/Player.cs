using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace PixelPortal
{
    public class Player : PhysicalEntity
    {
        public Player(PortalHandler portalSystem, Tilemap tilemap, float collisionradious = 10) : base( portalSystem, tilemap, collisionradious)
        {
        }
        public Player(PortalHandler portalSystem, Tilemap tilemap, Texture2D texture = null, AnimatedSprite animatedSprite = null, float collisionradious = 10, Vector2? position = null, float rotation = 0, float scale = 1, SpriteEffects spriteEffects = SpriteEffects.None, float layerDepth = 0)
        : base(portalSystem, tilemap, texture, animatedSprite, collisionradious, position, rotation, scale, spriteEffects, layerDepth)
        {
         
        }

        public override void PhysicsUpdate(float delta)
        {
            MovementInput();
            base.PhysicsUpdate(delta);
        }

        public void MovementInput()
        {
            Input i = Game1.input;
            if(i.IsKeyDown(Keys.D))
            {
                velocity.X += 10;
            }
            if (i.IsKeyDown(Keys.A))
            {
                velocity.X -= 10;
            }
            if (i.IsKeyJustPressed(Keys.Space))
            {
                velocity.Y -= 500;
            }
        }
    }
}
