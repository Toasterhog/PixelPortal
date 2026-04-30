using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace MGSimpelFysik
{
   
    public class Tilemap : IDrawable
    {
        private int[,] tiles = new int[20, 12];
        public int[,] Tiles => tiles;
        private static int tileSize = 50;
        public static int TileSize { get { return tileSize; } set { tileSize = value; } }
        private Texture2D tileset;
        private Rectangle[] sourceRects;
        public AnimatedSprite goalsprite;
        public Texture2D lightLayer;

        public Tilemap(Texture2D _tileSet, int _tileSetSourceSize = -1)
        {
            tileset = _tileSet;
            int tileSetSourceSize = _tileSetSourceSize > 0 ? _tileSetSourceSize : _tileSet.Height ; //assume square
            sourceRects = new Rectangle[tileset.Width / tileSetSourceSize];
            for (int i = 0; i * tileSetSourceSize < tileset.Width; i++)
            {
                sourceRects[i] = new Rectangle(i * tileSetSourceSize, 0, tileSetSourceSize, tileset.Height);
            }
        }

        public static Point PosToTile(Vector2 pos)
        {
            return new Point((int)pos.X / tileSize, (int)pos.Y / tileSize);
        }
        public static Vector2 TileTOPos(Point coord)
        {
            return new Vector2(coord.X * tileSize, coord.Y * tileSize);
        }
        public static Vector2 TileTOPosCenter(Point coord)
        {
            return new Vector2(coord.X * tileSize, coord.Y * tileSize) + new Vector2(tileSize / 2f, tileSize / 2f);
        }
        public static Point TileTOPosCenterP(Point coord) //needs even tileSize
        {
            return new Point(coord.X * tileSize, coord.Y * tileSize) + new Point(tileSize / 2, tileSize / 2);
        }


        public int GetTileType(Vector2 pos)
        {
            Point coord = PosToTile(pos);
            return GetTileType(coord);
        }
        public int GetTileType(Point coord)
        {
            coord = Mathlike.WrapP(coord, new Point(20, 12));
            return tiles[coord.X, coord.Y];
        }
        //public bool GetTileCollision(Point coord)
        //{
        //    int tileType = GetTileType(coord);
        //    return
        //}
        public void SetTiles(int[,] newTiles)
        {
            if (tiles.GetLength(0) == newTiles.GetLength(0) && tiles.GetLength(1) == newTiles.GetLength(1))
            {
                tiles = newTiles;
            }
        }
        public Point GetTileMapSize() { return new Point(tiles.GetLength(0), tiles.GetLength(1)); }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    int tile = tiles[x, y];
                    if (tile >= 0 && tile < sourceRects.Length)
                    {
                        if(tile == 3)
                        {
                            spriteBatch.Draw(goalsprite.texture, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), goalsprite.CurrentTextureRegion, Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(tileset, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), sourceRects[tile], Color.White);
                        }


                        //int sourceIndex = 0;
                        //if (GetTileType(new Point(x, y - 1)) >= 0) sourceIndex += 1;
                        //if (GetTileType(new Point(x + 1, y)) >= 0) sourceIndex += 2;
                        //if (GetTileType(new Point(x, y + 1)) >= 0) sourceIndex += 4;
                        //if (GetTileType(new Point(x - 1, y)) >= 0) sourceIndex += 8;
                        //int ts = 32;
                        //spriteBatch.Draw(lightLayer, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), new Rectangle(sourceIndex*ts,0,ts,ts), Color.White);

                        // 1. Calculate the 8-bit bitmask index
                        int sourceIndex = 0;
                        // Bitmask order matches the generation logic: 0:N, 1:NE, 2:E, 3:SE, 4:S, 5:SW, 6:W, 7:NW
                        if (GetTileType(new Point(x, y - 1)) >= 0) sourceIndex += 1;   // N
                        if (GetTileType(new Point(x + 1, y - 1)) >= 0) sourceIndex += 2;   // NE
                        if (GetTileType(new Point(x + 1, y)) >= 0) sourceIndex += 4;   // E
                        if (GetTileType(new Point(x + 1, y + 1)) >= 0) sourceIndex += 8;   // SE
                        if (GetTileType(new Point(x, y + 1)) >= 0) sourceIndex += 16;  // S
                        if (GetTileType(new Point(x - 1, y + 1)) >= 0) sourceIndex += 32;  // SW
                        if (GetTileType(new Point(x - 1, y)) >= 0) sourceIndex += 64;  // W
                        if (GetTileType(new Point(x - 1, y - 1)) >= 0) sourceIndex += 128; // NW

                        // 2. Convert index to 16x16 grid coordinates
                        int ts = 32;
                        int gridX = (sourceIndex % 16) * ts;
                        int gridY = (sourceIndex / 16) * ts;

                        // 3. Draw using the calculated source rectangle
                        spriteBatch.Draw(lightLayer,
                            new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize),
                            new Rectangle(gridX, gridY, ts, ts),
                            Color.White);
                    }
                }
            }
        }

    }
}
