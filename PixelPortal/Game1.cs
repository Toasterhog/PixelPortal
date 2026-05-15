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

        private int windowWidth = 20 * 80; //1000;
        private int windowHeight = 12 * 80; //600;

        private KeyboardState prevks;
        private MouseState prevms;
        //private Point blueAimStart; //för att skuta projektilerna med alternativa inputvarianten
        //private Point yellowAimStart;

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
        private Texture2D backgroundTexture;
        private Texture2D hämisTexture;

        public Effect portalGlowShader;

        private AnimatedSprite goalAnim;
        private AdvancedSprite playerAnim;
        public AdvancedSprite blueProjectileAnim;
        public AdvancedSprite yellowProjectileAnim;

        private LevelBuilder levelBuilder;
        private Player goomba;
        private PhysicalEntity companionCube;
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

            tileSetTexture = Content.Load<Texture2D>("tileset_16x1_8px");
            dungeonTexture = Content.Load<Texture2D>("dungeon");
            goombaTexture = Content.Load<Texture2D>("goomba");
            companionCubeTexture = Content.Load<Texture2D>("companionCube");
            goalTexture = Content.Load<Texture2D>("goalblob_8px");
            blueProjectileTexture = Content.Load<Texture2D>("projectile2");
            bluePortalFrameTexture = Content.Load<Texture2D>("portalGlow");
            lightTileSetTexture = Content.Load<Texture2D>("CLTS");
            backgroundTexture = Content.Load<Texture2D>("pabackground_blue-gray");//background här
            hämisTexture = Content.Load<Texture2D>("hämis_spritesheet_white"); //hämis

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

            playerAnim = new AdvancedSprite(hämisTexture, new Point(12, 12), new int[] { 10, 12, 4, 5 });
            playerAnim.Delay = 1000 / 10f;

            tilemap = new Tilemap(tileSetTexture); //had param 8
            tilemap.goalsprite = goalAnim;
            tilemap.lightLayer = lightTileSetTexture;

            goomba = new Player(portalSystem, tilemap, null, playerAnim, 80 / 8 * 3f, position: new Vector2(600,300), scale: 80/8 * 6/10f ); //hämis textur är 12 men 1px kant är utan kollision = 10
            goomba.origin = new Vector2(6, 6);
            goomba.colorMultiplier = Color.FromNonPremultiplied(195,130,220,255); //player color (255,250,140,255)

            companionCube = new PhysicalEntity(portalSystem, tilemap, companionCubeTexture, null, 40f, position: new Vector2(300, 300), scale: 80f / 134f);
            //companionCube = new PhysicalEntity(portalSystem, tilemap, companionCubeTexture, null, 25f, position: new Vector2(300, 300), scale: 50f / 8f);

            physicsWorld = new Physics(windowWidth, windowHeight);
            physicsWorld.entities.Add(goomba);
            physicsWorld.entities.Add(companionCube);
            levelBuilder = new LevelBuilder(tilemap, dungeonTexture);


            levelBuilder.SetTilesFromImage(GraphicsDevice, tilemap);
        }



        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            input.UpdateNoDelta();
            InputUppdate();

            playerAnim.Update(gameTime);
            goalAnim.Update(gameTime);
            blueProjectileAnim.Update(gameTime);
            yellowProjectileAnim.Update(gameTime);

            physicsWorld.Update(gameTime);
            SoundHandler.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.FromNonPremultiplied(140,110,60,255));
            //GraphicsDevice.Clear(Color.FromNonPremultiplied(16, 16, 16, 255));
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            //_spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, windowWidth, windowHeight), Color.DarkGray);// sourceRectangle: new Rectangle(70,50,200,120));
            _spriteBatch.Draw(
                   backgroundTexture,
                   new Rectangle(0, 0, windowWidth, windowHeight),
                   new Rectangle(170, 50, 200, 120),
                   Color.DarkGray,
                   0,
                   Vector2.One,
                   SpriteEffects.None,
                   0
               );


            goomba.Draw(_spriteBatch);
            companionCube.Draw(_spriteBatch);
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

            if (ks.IsKeyDown(Keys.M)) //move goomba cheat
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
            if (ks.IsKeyDown(Keys.U) && prevks.IsKeyUp(Keys.U))
            {
                Point tileMapSize = tilemap.GetTileMapSize();
                int ts = Tilemap.tileSize == 80 ? 50 : 80;
                ChangeTileSize(ts);
                ChangeWindowSize(tileMapSize.X * ts, tileMapSize.Y * ts);
            }
            

            prevks = ks;
            prevms = ms;
        }

        //public void ToggleFullscreen()
        //{
        //    //Window.IsBorderless = !Window.IsBorderless;
        //    //_graphics.IsFullScreen = !_graphics.IsFullScreen;

        //    _graphics.ApplyChanges();
        //    int w = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        //    int h = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        //    ChangeWindowSize(w, h);

        //    physicsWorld.deltaMultiplier = 8 / 5f; ;
        //}
        public void ChangeWindowSize(int w, int h)
        {
            w = Mathlike.ClampI(w, 100, 2000);
            h = Mathlike.ClampI(h, 100, 2000);
            _graphics.PreferredBackBufferWidth = w;
            _graphics.PreferredBackBufferHeight = h;
            _graphics.ApplyChanges();
            windowWidth = w;
            windowHeight = h;

        }
        public void ChangeTileSize(int ts)
        {
            Tilemap.tileSize = ts;
            if (physicsWorld != null)
            {
                foreach (PhysicalEntity pe in physicsWorld.entities) { pe.tileSize = ts; }

                Point tileMapSize = tilemap.GetTileMapSize();
                physicsWorld.worldWidth = ts * tileMapSize.X;
                physicsWorld.worldHeight = ts * tileMapSize.Y;
            }
        }

        

    }




    
    public static class TemporaryStuff
    {
        //flyta till game1 för att använda, behöver GraphicsDevice
        
        public static void SaveTextureToFile(Texture2D texture, string fileName)
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
        public static Texture2D CreateLightTileSetOld(GraphicsDevice GD) //sista itteration LTS med gpt, gammal
        {
            int ts = 32;
            int gridWidth = 16;
            int gridHeight = 16;

            int texWidth = ts * gridWidth;
            int texHeight = ts * gridHeight;

            Texture2D mytex = new Texture2D(GD, texWidth, texHeight);
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

        public static Texture2D CreateDebugLightTileSet(GraphicsDevice GD) //sista itteration debugLTS med gemini
        {
            int subTs = 9;
            int innerTs = subTs * 3;
            int ts = innerTs + 1;
            int gridWidth = 16;
            int gridHeight = 16;

            int texWidth = ts * gridWidth;
            int texHeight = ts * gridHeight;

            Texture2D mytex = new Texture2D(GD, texWidth, texHeight);
            Color[] colData = new Color[texWidth * texHeight];

            int[,] bitMap = new int[3, 3] {
            { 7,  0,  1 },
            { 6, -1,  2 },
            { 5,  4,  3 }};

            for (int tile = 0; tile < 256; tile++)
            {
                bool[] hasaa = new bool[4];
                for (int i = 0; i < 4; i++) hasaa[i] = (tile & (1 << 2 * i)) != 0;
                bool[] hasdiag = new bool[4];
                for (int i = 0; i < 4; i++) hasdiag[i] = (tile & (1 << (1 + 2 * i))) != 0;

                int tileX = (tile % gridWidth) * ts;
                int tileY = (tile / gridWidth) * ts;

                for (int y = 0; y < ts; y++)
                {
                    for (int x = 0; x < ts; x++)
                    {
                        int pixelX = tileX + x;
                        int pixelY = tileY + y;
                        int index = (pixelY * texWidth) + pixelX;

                        // Rutnätslinjen
                        if (x == innerTs || y == innerTs)
                        {
                            colData[index] = Color.DimGray;
                            continue;
                        }

                        int cellX = x / subTs;
                        int cellY = y / subTs;

                        if (cellX == 1 && cellY == 1)
                        {
                            // --- MITTEN ---
                            int localX = x - subTs;
                            int localY = y - subTs;

                            float minDistance = 1.0f;
                            float maxP = subTs - 1f;

                            // 1. Raka kanter
                            if (!hasaa[0]) minDistance = Math.Min(minDistance, localY / maxP);
                            if (!hasaa[1]) minDistance = Math.Min(minDistance, (maxP - localX) / maxP);
                            if (!hasaa[2]) minDistance = Math.Min(minDistance, (maxP - localY) / maxP);
                            if (!hasaa[3]) minDistance = Math.Min(minDistance, localX / maxP);

                            // 2. Diagonala hörn
                            if (!hasdiag[3]) minDistance = Math.Min(minDistance, (float)Math.Sqrt(localX * localX + localY * localY) / maxP);
                            if (!hasdiag[0]) minDistance = Math.Min(minDistance, (float)Math.Sqrt((maxP - localX) * (maxP - localX) + localY * localY) / maxP);
                            if (!hasdiag[1]) minDistance = Math.Min(minDistance, (float)Math.Sqrt((maxP - localX) * (maxP - localX) + (maxP - localY) * (maxP - localY)) / maxP);
                            if (!hasdiag[2]) minDistance = Math.Min(minDistance, (float)Math.Sqrt(localX * localX + (maxP - localY) * (maxP - localY)) / maxP);

                            minDistance = MathHelper.Clamp(minDistance, 0f, 1f);

                            // --- NY STYLISERING ---
                            float lerpAmount;
                            if (minDistance <= 0.5f)
                            {
                                // Om vi är närmare kanten än 0.5 tiles, var helt ljus (0.0 i Lerp)
                                lerpAmount = 0f;
                            }
                            else
                            {
                                // Om vi är mellan 0.5 och 1.0 tiles ifrån kanten, mappa om det till 0.0 - 1.0
                                // Exempel: 0.5 blir 0.0 (ljus), 1.0 blir 1.0 (mörk)
                                lerpAmount = (minDistance - 0.5f) * 2f;
                            }

                            // Applicera färgen med det nya styliserade värdet
                            colData[index] = Color.Lerp(Color.Yellow, Color.DarkBlue, lerpAmount);
                        }
                        else
                        {
                            // --- INDIKATORERNA ---
                            int bitIndex = bitMap[cellY, cellX];
                            bool isBitActive = (tile & (1 << bitIndex)) != 0;
                            colData[index] = isBitActive ? Color.Black : Color.White;
                        }
                    }
                }
            }

            mytex.SetData(colData);
            return mytex;
        }

        public static Texture2D CreateLightTileSet(GraphicsDevice GD) //slutliga light tileset med gemini
        {
            int ts = 32; // kan välja tilestorlek
            int gridWidth = 16;
            int gridHeight = 16;

            int texWidth = ts * gridWidth; // 128
            int texHeight = ts * gridHeight; // 128

            Texture2D mytex = new Texture2D(GD, texWidth, texHeight);
            Color[] colData = new Color[texWidth * texHeight];

            for (int tile = 0; tile < 256; tile++)
            {
                bool[] hasaa = new bool[4];
                for (int i = 0; i < 4; i++) hasaa[i] = (tile & (1 << 2 * i)) != 0;

                bool[] hasdiag = new bool[4];
                for (int i = 0; i < 4; i++) hasdiag[i] = (tile & (1 << (1 + 2 * i))) != 0;

                int tileX = (tile % gridWidth) * ts;
                int tileY = (tile / gridWidth) * ts;

                for (int y = 0; y < ts; y++)
                {
                    for (int x = 0; x < ts; x++)
                    {
                        int pixelX = tileX + x;
                        int pixelY = tileY + y;
                        int index = (pixelY * texWidth) + pixelX;

                        float minDistance = 1.0f;
                        float maxP = ts - 1f; // ts är 8, så maxP blir 7f
                        
                        // 1. Avstånd till raka kanter
                        if (!hasaa[0]) minDistance = Math.Min(minDistance, y / maxP);             // Top
                        if (!hasaa[1]) minDistance = Math.Min(minDistance, (maxP - x) / maxP);    // Right
                        if (!hasaa[2]) minDistance = Math.Min(minDistance, (maxP - y) / maxP);    // Bottom
                        if (!hasaa[3]) minDistance = Math.Min(minDistance, x / maxP);             // Left

                        // 2. Avstånd till hörn (Börjar Top-Right och går medurs)
                        if (!hasdiag[0]) minDistance = Math.Min(minDistance, (float)Math.Sqrt((maxP - x) * (maxP - x) + y * y) / maxP);                   // Top-Right
                        if (!hasdiag[1]) minDistance = Math.Min(minDistance, (float)Math.Sqrt((maxP - x) * (maxP - x) + (maxP - y) * (maxP - y)) / maxP); // Bottom-Right
                        if (!hasdiag[2]) minDistance = Math.Min(minDistance, (float)Math.Sqrt(x * x + (maxP - y) * (maxP - y)) / maxP);                   // Bottom-Left
                        if (!hasdiag[3]) minDistance = Math.Min(minDistance, (float)Math.Sqrt(x * x + y * y) / maxP);                                     // Top-Left
                        
                        minDistance = MathHelper.Clamp(minDistance, 0f, 1f);

                        // 3. Applicera den styliserade tröskeln
                        float lerpAmount;
                        if (minDistance <= 0.5f)
                        {
                            // De yttersta 50% av tilen (nära luften)
                            lerpAmount = 0f;
                        }
                        else
                        {
                            // De inre 50% skapar övergången till djupt mörker
                            lerpAmount = (minDistance - 0.5f) * 2f;
                        }
                        
                        // I MonoGame styr multiplikation med en float alfakanalen.
                        // lerpAmount = 0.0 blir helt transparent.
                        // lerpAmount = 1.0 blir 100% svart.
                        colData[index] = Color.Black * lerpAmount;
                    }
                }
            }

            mytex.SetData(colData);

            // Spara ner din textur för att kunna använda den i spelet!
            // SaveTextureToFile(mytex, "StylizedAO_TileSet"); 

            return mytex;
        }

        public static Texture2D CreateCompactShadowTileSet(GraphicsDevice GD) //helt annan inplementation, 4x4 istället för 16x16, faktiskt slutliga
        {
            int ts = 32;
            int texWidth = ts * 4;  // 4 kolumner
            int texHeight = ts * 4; // 4 rader

            Texture2D mytex = new Texture2D(GD, texWidth, texHeight);
            Color[] colData = new Color[texWidth * texHeight];

            for (int gridY = 0; gridY < 4; gridY++)
            {
                for (int gridX = 0; gridX < 4; gridX++)
                {
                    for (int y = 0; y < ts; y++)
                    {
                        for (int x = 0; x < ts; x++)
                        {
                            int pixelX = (gridX * ts) + x;
                            int pixelY = (gridY * ts) + y;
                            int index = (pixelY * texWidth) + pixelX;

                            float maxP = ts - 1f;
                            float nx = x / maxP; // Normaliserad X (0.0 - 1.0)
                            float ny = y / maxP; // Normaliserad Y (0.0 - 1.0)

                            float alpha = 0f; // 0 = Ljust, 1 = Mörkt

                            // --- Rad 0: Special ---
                            if (gridY == 0)
                            {
                                if (gridX == 0) alpha = 0f; // Helt ljus
                                if (gridX == 1) alpha = 1f; // Helt mörk
                                if (gridX == 3)
                                {
                                    // Ljus i NV och SÖ (Diagonalt)
                                    float alphaNW = (float)Math.Sqrt(nx * nx + ny * ny);
                                    float alphaSE = (float)Math.Sqrt((1f - nx) * (1f - nx) + (1f - ny) * (1f - ny));
                                    alpha = 1 - Math.Min(1f, Math.Min(alphaNW, alphaSE));
                                }
                                if (gridX == 2)
                                {
                                    // Ljus i NÖ och SV (Diagonalt)
                                    float alphaNE = (float)Math.Sqrt((1f - nx) * (1f - nx) + ny * ny);
                                    float alphaSW = (float)Math.Sqrt(nx * nx + (1f - ny) * (1f - ny));
                                    alpha = 1 - Math.Min(1f, Math.Min(alphaNE, alphaSW));
                                }
                            }
                            // --- Rad 1: Linjära väggar ---
                            else if (gridY == 1)
                            {
                                if (gridX == 0) alpha = ny;            // Luft Norr
                                if (gridX == 1) alpha = 1f - nx;       // Luft Öst
                                if (gridX == 2) alpha = 1f - ny;       // Luft Söder
                                if (gridX == 3) alpha = nx;            // Luft Väst
                            }
                            // --- Rad 2: Ljusa hörn (Konvexa hörn där 2 intilliggande sidor är luft) ---
                            else if (gridY == 2)
                            {
                                if (gridX == 0) alpha = 1f - Math.Min(1f, (float)Math.Sqrt(nx * nx + (1f - ny) * (1f - ny)));             // Luft N & Ö
                                if (gridX == 1) alpha = 1f - Math.Min(1f, (float)Math.Sqrt(nx * nx + ny * ny));                           // Luft S & Ö
                                if (gridX == 2) alpha = 1f - Math.Min(1f, (float)Math.Sqrt((1f - nx) * (1f - nx) + ny * ny));             // Luft S & V
                                if (gridX == 3) alpha = 1f - Math.Min(1f, (float)Math.Sqrt((1f - nx) * (1f - nx) + (1f - ny) * (1f - ny))); // Luft N & V
                            }
                            // --- Rad 3: Mörka hörn (Konkava hörn där luften BARA nuddar diagonalt) ---
                            else if (gridY == 3)
                            {
                                if (gridX == 0) alpha = Math.Min(1f, (float)Math.Sqrt((1f - nx) * (1f - nx) + ny * ny));               // Luft NÖ
                                if (gridX == 1) alpha = Math.Min(1f, (float)Math.Sqrt((1f - nx) * (1f - nx) + (1f - ny) * (1f - ny))); // Luft SÖ
                                if (gridX == 2) alpha = Math.Min(1f, (float)Math.Sqrt(nx * nx + (1f - ny) * (1f - ny)));               // Luft SV
                                if (gridX == 3) alpha = Math.Min(1f, (float)Math.Sqrt(nx * nx + ny * ny));                             // Luft NV
                            }

                            colData[index] = Color.Black * alpha;
                        }
                    }
                }
            }

            mytex.SetData(colData);
            return mytex;
        }

        //från innit() eller loadC() //TemporaryStuff.SaveTextureToFile(TemporaryStuff.CreateCompactShadowTileSet(GraphicsDevice), "CLTS0");
    }

}
