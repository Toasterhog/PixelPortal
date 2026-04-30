using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace PixelPortal
{
    public class Entity : IDrawable
    {
        public Vector2 position = new Vector2(50,50);
        public float rotation = 0;
        public Vector2 origin = Vector2.Zero;
        public float scale = 1;
        public Texture2D texture;
        public AnimatedSprite animatedSprite;
        public SpriteEffects spriteEffects = SpriteEffects.None;
        public float layerDepth = 0;
        public Color colorMultiplier = Color.White;


        public Entity(Texture2D texture = null, AnimatedSprite animatedSprite = null, Vector2? position = null, float rotation = 0, float scale = 1, SpriteEffects spriteEffects = SpriteEffects.None, float layerDepth = 0)
        {
            this.texture = texture;
            this.position = position ?? new Vector2(100, 100);
            this.rotation = rotation;
            this.scale = scale;
            this.animatedSprite = animatedSprite;
            this.spriteEffects = spriteEffects;
            this.layerDepth = layerDepth;
            origin = animatedSprite != null ? new Vector2(animatedSprite.CurrentTextureRegion.Width, animatedSprite.CurrentTextureRegion.Height) / 2f :
            texture != null ? new Vector2(texture.Width, texture.Height) / 2f : 
            Vector2.Zero;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
            if (animatedSprite != null)
            {
                spriteBatch.Draw(animatedSprite.texture, position, animatedSprite.CurrentTextureRegion, colorMultiplier, rotation, origin, scale, spriteEffects, layerDepth);
            }
            else if (texture != null) 
            {
                spriteBatch.Draw(texture, position, null, colorMultiplier, rotation, origin, scale, spriteEffects, layerDepth);
            }
        }
    }
  
}