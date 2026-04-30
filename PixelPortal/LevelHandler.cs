using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace MGSimpelFysik
{
    public class LevelBuilder
    {
        public Texture2D fallBackTexture;
        private Tilemap tilemap;
        private readonly (int ID, Color Col)[] Palette = new[]{
            ( -1, Color.Black ),
            (  0, Color.Gray ),
            (  0, Color.Brown ),
            (  1, Color.White ),
            (  1, Color.LightGray ),
            (  2, Color.Red ),
            (  3, Color.Blue ),
            (  4, Color.Green )};

        public LevelBuilder(Tilemap tilemap, Texture2D fallBackTexture)
        {
            this.fallBackTexture = fallBackTexture;
            this.tilemap = tilemap;
        }


        public Texture2D GetLevelImage(GraphicsDevice GD) //todo maybe be able to load multiple images/levels
        {
            try
            {
                string appDataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                string gameFolder = Path.Combine(appDataPath, "MonoGameTileMapTest");

                if (!Directory.Exists("MonoGameTileMapTest"))
                {
                    Directory.CreateDirectory("MonoGameTileMapTest");
                }

                string levelImgPath = Path.Combine(gameFolder, "level1.png");
                if (!File.Exists(levelImgPath))
                {
                    Debug.WriteLine("levelhandler level1 not exist");
                    Debug.WriteLine(levelImgPath);
                    return null;
                }
                Texture2D tex;
                using (FileStream stream = new FileStream(levelImgPath, FileMode.Open))
                {
                    tex = Texture2D.FromStream(GD, stream);
                }
                return tex;
            }
            catch { Debug.WriteLine("levelhandler error"); }
            return null;
        }

        public void SetTilesFromImage(GraphicsDevice GD, Tilemap tilemap)
        {
            Texture2D tileMapTexture = GetLevelImage(GD) ?? fallBackTexture;
            if (tileMapTexture == null) { Debug.WriteLine("LevelBuilder cannot find image and has no falbacktexture"); return; }
            Color[] colorData = new Color[tileMapTexture.Width * tileMapTexture.Height];
            tileMapTexture.GetData<Color>(colorData);

            Point tilemapSize = tilemap.GetTileMapSize();
            int[,] tempTiles = new int[tilemapSize.X, tilemapSize.Y];
            for (int y = 0; y < tilemapSize.Y; y++)
            {
                for (int x = 0; x < tilemapSize.X; x++)
                {
                    tempTiles[x, y] = ColorToTileType(colorData[x + tilemapSize.X * y]);
                }
            }
            tilemap.SetTiles(tempTiles);
        }

        //private int ColorToTileType(Color color)
        //{
        //    //return color.R < 128 ? 0 : -1;
        //    if(color.G > 200) return -1;
        //    if (color.R + color.B < 10) return 0;
        //    if (color.B  > color.G + color.R) return 1;
        //    if (color.R  > color.B + color.G) return 2;
        //    return -1;
        //}

        private int ColorToTileType(Color color)
        {
            if (color.A < 255) return -1;
            else
            {
                int sum = color.R + color.G + color.B;
                if (sum == 0) return -1;
                if (sum > 750) return 1;
            }
            

            int closestID = -1;
            float minDistance = float.MaxValue;

            for (int i = 0; i < Palette.Length; i++)
            {
                float dr = color.R - Palette[i].Col.R;
                float dg = color.G - Palette[i].Col.G;
                float db = color.B - Palette[i].Col.B;
                float distanceSquared = (dr * dr) + (dg * dg) + (db * db);

                if (distanceSquared < minDistance)
                {
                    minDistance = distanceSquared;
                    closestID = Palette[i].ID;
                }
            }
            return closestID;
        }



    }
}
