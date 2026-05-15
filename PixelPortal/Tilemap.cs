using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct2D1.Effects;
using System.Diagnostics;

namespace PixelPortal
{
   
    public class Tilemap : IDrawable
    {
        private int[,] tiles = new int[20, 12];
        public int[,] Tiles => tiles;
        public static int tileSize = 80;
         
        private Texture2D tileset;
        private Rectangle[] sourceRects;
        public AnimatedSprite goalsprite;

        public Texture2D lightLayer;
        private int[,] lightMap = new int[20, 12];

        public Tilemap(Texture2D _tileSet, int _tileSetSourceSize = -1)
        {
            tileset = _tileSet;
            int tileSetSourceSize = _tileSetSourceSize > 0 ? _tileSetSourceSize : _tileSet.Height ; 
            sourceRects = new Rectangle[tileset.Width / tileSetSourceSize];
            for (int i = 0; i * tileSetSourceSize < tileset.Width; i++)
            {
                sourceRects[i] = new Rectangle(i * tileSetSourceSize, 0, tileSetSourceSize, tileSetSourceSize);
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

        public bool GetTileCollision(Point coord)
        {
            return TileIsSolid(GetTileType(coord));
        }
        public bool TileIsSolid(int tileType)
        {
            return tileType >= 0 && tileType < 4;
        }

        public bool TileIsPortalPlacable(int tileType)
        {
            return tileType == 0;
        }

        public void SetTiles(int[,] newTiles)
        {
            if (tiles.GetLength(0) == newTiles.GetLength(0) && tiles.GetLength(1) == newTiles.GetLength(1))
            {
                tiles = newTiles;
            }
            UpdateLightMap();
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
                        spriteBatch.Draw(tileset, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), sourceRects[tile], Color.White);
                        

                        int LTSindex = lightMap[x, y];
                        int ts = 32;
                        spriteBatch.Draw(lightLayer,
                            new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize),
                            new Rectangle(LTSindex % 4 * ts, LTSindex / 4 * ts, ts, ts),
                            Color.White*0.95f
                        );
                    }
                    else if (tile == 16)
                    {
                        spriteBatch.Draw(goalsprite.texture, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), goalsprite.CurrentTextureRegion, Color.White);
                    }
                }
            }
        }
        #region light tile map
        public void UpdateLightMap()
        {
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    int tile = tiles[x, y];
                    if (TileIsSolid(tile))
                    {
                        Point LTSCoord = LTSRect(x, y);
                        int LTSindex = LTSCoord.X + LTSCoord.Y * 4;
                        lightMap[x, y] = LTSindex;

                    }
                }
            }
            Debug.WriteLine(lightMap[10,5].ToString());
        }

        private Point LTSRect(int x, int y)
        {
            Point tpos = new Point(x, y);
            Point[] axisAlignedOffsets = [new Point(0, -1), new Point(1, 0), new Point(0, 1), new Point(-1, 0)];
            Point[] diagonalOffsets = [new Point(1, -1), new Point(1, 1), new Point(-1, 1), new Point(-1, -1)];
            bool[] corner = { false, false, false, false };

            for (int i = 0; i < 4; i++)
            {
                if (!TileIsSolid( GetTileTypeDefault(tpos + axisAlignedOffsets[i], 1)))
                {
                    corner[i] = true;
                    corner[Mathlike.ModI(i - 1, 4)] = true;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (!TileIsSolid(GetTileTypeDefault(tpos + diagonalOffsets[i], 1)))
                {
                    corner[i] = true;
                }
            }
            int sumLitCorners = 0;
            foreach (bool c in corner)
            {
                if (c) sumLitCorners++;
            }
            if (sumLitCorners == 4) return Point.Zero;
            if (sumLitCorners == 3)
            {
                if (!corner[0]) return new Point(2, 2);
                if (!corner[1]) return new Point(3, 2);
                if (!corner[2]) return new Point(0, 2);
                if (!corner[3]) return new Point(1, 2);
            }
            if (sumLitCorners == 2)
            {
                if (corner[3] && corner[0]) return new Point(0, 1);
                if (corner[0] && corner[1]) return new Point(1, 1);
                if (corner[1] && corner[2]) return new Point(2, 1);
                if (corner[2] && corner[3]) return new Point(3, 1);

                if (corner[0] && corner[2]) return new Point(3, 0);
                if (corner[1] && corner[3]) return new Point(2, 0);
            }
            if (sumLitCorners == 1)
            {
                if (corner[0]) return new Point(0, 3);
                if (corner[1]) return new Point(1, 3);
                if (corner[2]) return new Point(2, 3);
                if (corner[3]) return new Point(3, 3);
            }
            return new Point(1, 0);
        }
        public int GetTileTypeDefault(Point coord, int defaul = 1)
        {
            Point coordWraped = Mathlike.WrapP(coord, new Point(20, 12));
            if (coord != coordWraped) return defaul;
            return tiles[coord.X, coord.Y];
        }
        #endregion 
    }
}
