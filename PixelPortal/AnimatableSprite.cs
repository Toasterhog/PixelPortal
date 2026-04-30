using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;


namespace MGSimpelFysik
{
    public class AnimatedSprite
    {
        public Texture2D texture;
        private Rectangle[] textureRegions;
        protected TimeSpan elapsed = TimeSpan.Zero;
        protected TimeSpan delay = TimeSpan.FromMilliseconds(200);
        public double Delay { get { return delay.TotalMilliseconds; } set { delay = TimeSpan.FromMilliseconds(value); } }
        public bool Playing = true;
        private int frame = 0;
        public int Frame {  
            get { return frame; } 
            set {
                if (value > textureRegions.Length) {frame = textureRegions.Length;}
                else if (value < 0) { frame = 0; }
                else { frame = value; }
            } 
        }
        public virtual Rectangle CurrentTextureRegion => textureRegions[frame];

        public AnimatedSprite(Texture2D texture) 
        { 
            this.texture = texture; 
            //Playing = false; 
        }
        public AnimatedSprite(Texture2D Texture, int regionWidth = 0)
        {
            if (regionWidth <= 0) regionWidth = texture.Height;
            texture = Texture;
            textureRegions = new Rectangle[Texture.Width / regionWidth];
            for (int i = 0; i * regionWidth < texture.Width; i++)
            {
                textureRegions[i] = new Rectangle(i * regionWidth, 0, regionWidth, texture.Height);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!Playing) return;

            elapsed += gameTime.ElapsedGameTime;
            if (elapsed >= delay)
            {
                elapsed -= delay;
                frame++;
                if (frame >= textureRegions.Length)
                {
                    frame = 0;
                }

            }
        }

    }
    public class AdvancedSprite : AnimatedSprite
    {
        private Rectangle[,] textures;
        private Point index = new Point();
        public override Rectangle CurrentTextureRegion => textures[index.X,index.Y];

        public AdvancedSprite(Texture2D texture, Point regionSize) : base(texture)
        {
            textures = new Rectangle[texture.Width/regionSize.X , texture.Height/regionSize.Y];
            for ( int y = 0; y < textures.GetLength(1); y++)
            {
                for ( int x = 0; x < textures.GetLength(0); x++)
                {
                    textures[x, y] = new Rectangle(x * regionSize.X, y * regionSize.Y, regionSize.X, regionSize.Y);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!Playing) return;

            elapsed += gameTime.ElapsedGameTime;
            if (elapsed >= delay)
            {
                elapsed -= delay;
                index.X ++;
                if (index.X >= textures.GetLength(0))
                {
                    index.X = 0;
                }

            }
        }
        public void SetSequence(int sequence)
        {
            if (sequence >= 0 && sequence < textures.GetLength(1)) { index.Y = sequence; }
        }

    }
}
