using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace PixelPortal
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private int windowWidth = 1000;
        private int windowHeight = 600;

        private KeyboardState prevks;
        private MouseState prevms;
        private Point blueAimStart;
        private Point yellowAimStart;

        public SoundEffect shootSE;
        public SoundEffect openingPortalSE;

        private Texture2D tileSetTexture;
        private Texture2D dungeonTexture;
        private Texture2D goombaTexture;
        private Texture2D companionCubeTexture;
        private Texture2D goalTexture;
        private Texture2D blueProjectileTexture;
        //private Texture2D yellowProjectileTexture;
        public Texture2D bluePortalFrameTexture;
        public Texture2D lightTileSetTexture;

        public Effect portalGlowShader;

        private AnimatedSprite goombaAnim;
        private AnimatedSprite goalAnim;
        public AdvancedSprite blueProjectileAnim;
        public AdvancedSprite yellowProjectileAnim;

        private LevelBuilder levelBuilder;
        private Player goomba;
        public Tilemap tilemap;
        public Physics physicsWorld;
        public PortalHandler portalSystem;
        public static Input input;

        public List<IDrawable> visuals = new List<IDrawable>();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = windowWidth;
            _graphics.PreferredBackBufferHeight = windowHeight;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            tileSetTexture = Content.Load<Texture2D>("tilemap_8px");
            dungeonTexture = Content.Load<Texture2D>("dungeon");
            goombaTexture = Content.Load<Texture2D>("goomba");
            companionCubeTexture = Content.Load<Texture2D>("companionCube");
            goalTexture = Content.Load<Texture2D>("goalblob_8px");
            blueProjectileTexture = Content.Load<Texture2D>("projectile2");
            //yellowProjectileTexture = Content.Load<Texture2D>("dungeon");
            bluePortalFrameTexture = Content.Load<Texture2D>("portalGlow");
            lightTileSetTexture = Content.Load<Texture2D>("lightTileSet");

            shootSE = Content.Load<SoundEffect>("sound/Menu_Select_01");
            openingPortalSE = Content.Load<SoundEffect>("sound/WarpDrive_00");

            portalGlowShader = Content.Load<Effect>("shaders/portalGlow");
        }

        protected override void Initialize()
        {
            input = new Input();

            base.Initialize(); //base.init calls LoadContent

            SoundHandler.innitNoises(new SoundEffect[] { shootSE, openingPortalSE });
            portalSystem = new PortalHandler(this);
            blueProjectileAnim = new AdvancedSprite(blueProjectileTexture, new Point(16, 5));
            yellowProjectileAnim = new AdvancedSprite(blueProjectileTexture, new Point(16, 5));
            blueProjectileAnim.Delay = 50;
            yellowProjectileAnim.Delay = 35;
            goalAnim = new AnimatedSprite(goalTexture, 8);
            goalAnim.Delay = 120;
            tilemap = new Tilemap(tileSetTexture); //had param 8
            tilemap.goalsprite = goalAnim;
            tilemap.lightLayer = lightTileSetTexture; //new
            goombaAnim = new AnimatedSprite(goombaTexture, 16);
            //goomba = new PhysicalEntity(portalSystem, tilemap, null, goombaAnim, 16, position: new Vector2(300,300), scale: 4);
            goomba = new Player(portalSystem, tilemap, companionCubeTexture, null, 25f, position: new Vector2(300, 300), scale: 50f / 134f);
            //goomba.origin = new Vector2(8, 12);
            physicsWorld = new Physics(windowWidth, windowHeight);
            physicsWorld.entities.Add(goomba);
            levelBuilder = new LevelBuilder(tilemap, dungeonTexture);


            levelBuilder.SetTilesFromImage(GraphicsDevice, tilemap);
        }



        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            input.UpdateNoDelta();
            InputUppdate();

            goombaAnim.Update(gameTime);
            goalAnim.Update(gameTime);
            blueProjectileAnim.Update(gameTime);
            yellowProjectileAnim.Update(gameTime);

            physicsWorld.Update(gameTime);
            SoundHandler.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.FromNonPremultiplied(16, 16, 16, 255));
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            goomba.Draw(_spriteBatch);
            foreach (IDrawable visual in visuals)
            {
                visual.Draw(_spriteBatch);
            }
            tilemap.Draw(_spriteBatch);

            _spriteBatch.End();

            portalGlowShader.Parameters["time"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
            _spriteBatch.Begin(effect: portalGlowShader, blendState: BlendState.AlphaBlend); //, samplerState: SamplerState.PointClamp);
            portalSystem.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }



        private void InputUppdate()
        {
            KeyboardState ks = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

            if (ks.IsKeyDown(Keys.Space)) //move goomba cheat
            {
                if (ms.LeftButton == ButtonState.Pressed)
                {
                    goomba.position = new Vector2(ms.Position.X, ms.Position.Y);
                    goomba.velocity = Vector2.Zero;
                }
                else if (prevms.LeftButton == ButtonState.Pressed)
                {
                    goomba.position = new Vector2(ms.Position.X, ms.Position.Y);
                    goomba.velocity = new Vector2(ms.Position.X, ms.Position.Y) - new Vector2(prevms.Position.X, prevms.Position.Y);
                    goomba.velocity *= 100f;
                }
            }
            else
            {
                //----- direction from goomba ----
                if (prevms.LeftButton == ButtonState.Pressed && ms.LeftButton == ButtonState.Released)
                {

                    Vector2 dir = new Vector2(ms.Position.X, ms.Position.Y) - goomba.position;
                    portalSystem.SpawnProjectile(true, goomba.position, dir);
                    //Debug.WriteLine("blue proj spawned");

                }

                if (prevms.RightButton == ButtonState.Pressed && ms.RightButton == ButtonState.Released)
                {
                    Vector2 dir = new Vector2(ms.Position.X, ms.Position.Y) - goomba.position;
                    portalSystem.SpawnProjectile(false, goomba.position, dir);
                    //Debug.WriteLine("yellow proj spawned");

                }
            }

            //---- direction from drag press and hold ----
            //if (ms.LeftButton == ButtonState.Pressed && prevms.LeftButton == ButtonState.Released) { blueAimStart = ms.Position; }
            //else if (prevms.LeftButton == ButtonState.Pressed && ms.LeftButton == ButtonState.Released)
            //{
            //    Point pdir = ms.Position - blueAimStart;
            //    Vector2 dir = new Vector2(pdir.X, pdir.Y);
            //    portalSystem.SpawnProjectile(true, goomba.position, dir);
            //    Debug.WriteLine("blue proj spawned");
            //}

            //if (ms.RightButton == ButtonState.Pressed && prevms.RightButton == ButtonState.Released) { yellowAimStart = ms.Position; }
            //else if (prevms.RightButton == ButtonState.Pressed && ms.RightButton == ButtonState.Released)
            //{
            //    Point pdir = ms.Position - yellowAimStart;
            //    Vector2 dir = new Vector2(pdir.X, pdir.Y);
            //    portalSystem.SpawnProjectile(false, goomba.position, dir);
            //    Debug.WriteLine("yellow proj spawned");
            //}
            if (ks.IsKeyDown(Keys.U) && prevks.IsKeyUp(Keys.U)) {
                int ts = 64;
                Point tileMapSize = tilemap.GetTileMapSize();
                ChangeTileSize(ts);
                ChangeWindowSize(tileMapSize.X * ts, tileMapSize.Y * ts);
            }

            prevks = ks;
            prevms = ms;
        }

        public void ChangeWindowSize(int w, int h)
        {
            w = Mathlike.ClampI(w, 100, 2000);
            h = Mathlike.ClampI(h, 100, 2000);
            _graphics.PreferredBackBufferWidth = w;
            _graphics.PreferredBackBufferHeight = h;
            _graphics.ApplyChanges();

        }
        public void ChangeTileSize(int ts)
        {
            Tilemap.TileSize = ts;
            if (physicsWorld != null)
            {
                foreach (PhysicalEntity pe in physicsWorld.entities) { pe.tileSize = ts; }

                Point tileMapSize = tilemap.GetTileMapSize();
                physicsWorld.worldWidth = ts * tileMapSize.X;
                physicsWorld.worldHeight = ts * tileMapSize.Y;
            }
        }

        public void SaveTextureToFile(Texture2D texture, string fileName)
        {
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string gameFolder = Path.Combine(appDataPath, "MonoGameTileMapTest");

                // Ensure the directory exists
                if (!Directory.Exists(gameFolder))
                {
                    return;
                    //Directory.CreateDirectory(gameFolder);
                }

                string fullPath = Path.Combine(gameFolder, fileName + ".png");

                using (Stream stream = File.Create(fullPath))
                {
                    texture.SaveAsPng(stream, texture.Width, texture.Height);
                }

                Debug.WriteLine("Texture saved to: " + fullPath);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error saving texture: " + e.Message);
            }
        }
        private Texture2D CreateLightTileSet()
        {
            int ts = 32;
            int gridWidth = 16;
            int gridHeight = 16;

            int texWidth = ts * gridWidth;
            int texHeight = ts * gridHeight;

            Texture2D mytex = new Texture2D(GraphicsDevice, texWidth, texHeight);
            Color[] colData = new Color[texWidth * texHeight];

            for (int tile = 0; tile < 256; tile++)
            {
                bool[] hasaa = new bool[4]; //axis aligned
                for (int i = 0; i < 4; i++) hasaa[i] = (tile & (1 << 2 * i)) != 0;
                bool[] hasdiag = new bool[4]; //diagonal
                for (int i = 0; i < 4; i++) hasdiag[i] = (tile & (1 << (1 + 2 * i))) != 0;

                int tileX = (tile % gridWidth) * ts;
                int tileY = (tile / gridWidth) * ts;

                for (int y = 0; y < ts; y++)
                {
                    for (int x = 0; x < ts; x++)
                    {
                        float nx = (float)x / (ts - 1);
                        float ny = (float)y / (ts - 1);
                        float[] ndir = { 1 - ny, nx, ny, 1 - nx };
                        float light = 0f;
                        float liaghtDia = 0;

                        for (int i = 0; i < 4; i++)
                        {
                            if (!hasaa[i]) light += ndir[i];
                        }
                        for (int i = 0; i < 4; i++)
                        {
                            if (!hasdiag[i]) liaghtDia += ndir[i] * ndir[(i + 1) % 4];
                        }
                        light = MathHelper.Clamp(light, 0f, 1f);
                        light = Math.Max(light, liaghtDia);

                        //light = MathHelper.Clamp(light, 0f, 1f);


                        int pixelX = tileX + x;
                        int pixelY = tileY + y;
                        int index = (pixelY * texWidth) + pixelX;

                        colData[index] = Color.Black * (1 - light);
                    }
                }
            }

            mytex.SetData(colData);
            SaveTextureToFile(mytex, "lightTileSet");
            return mytex;
        }

    }

    public static class RandomInfo
    {
        public static readonly int windowWidth = 1000;
        public static readonly int windowHeight = 600;
        public static readonly int tileMapWidth = 20;
        public static readonly int tileMapHeight = 12;
        public static readonly int tileSize = 50;
    }
    

}
