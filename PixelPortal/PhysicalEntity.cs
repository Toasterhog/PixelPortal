using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MGSimpelFysik
{
    public class PhysicalEntity : Entity
    {
        public Vector2 velocity = Vector2.Zero; // px/s
        public Vector2 gravity = new Vector2(0, 800f); // 16 tiles per sekund^2 | px/s^2
        protected float collisionradious = 10;
        protected  Tilemap tilemap;
        protected enum CollisionShapeType { circle, square }; //meh datatyp
        protected CollisionShapeType colltype = CollisionShapeType.circle;
        protected float bounciness = 0.2f;
        protected float slidyness = 0.8f;
        protected float simulationSpeed = 1f;
        protected PortalHandler portalSys;

        public float tileSize = Tilemap.TileSize;
        protected Point[] axisAlignedOffsets = [new Point(1, 0), new Point(0, 1), new Point(-1, 0), new Point(0, -1)];
        protected Point[] diagonalOffsets = [new Point(1, 1), new Point(-1, 1), new Point(-1, -1), new Point(1, -1)];

        public PhysicalEntity(PortalHandler portalSystem, Tilemap tilemap, float collisionradious = 10) : base()
        {
            this.collisionradious = collisionradious;
            this.tilemap = tilemap;
            portalSys = portalSystem;
        }
        public PhysicalEntity(PortalHandler portalSystem, Tilemap tilemap, Texture2D texture = null, AnimatedSprite animatedSprite = null, float collisionradious = 10, Vector2? position = null, float rotation = 0, float scale = 1, SpriteEffects spriteEffects = SpriteEffects.None, float layerDepth = 0)
        : base(texture, animatedSprite, position, rotation, scale, spriteEffects, layerDepth)
        {
            this.collisionradious = collisionradious; //får inte vara mer än 25 (collrad/2)
            this.tilemap = tilemap;
            this.portalSys = portalSystem;
        }

        public void Update(GameTime gameTime)
        {

        }

        

        public virtual void PhysicsUpdate(float delta)
        {
            velocity += gravity * delta;
            ClampVelocity(delta);
            position += velocity * delta;

            Point tpos = Tilemap.PosToTile(position);


            if (tilemap.GetTileType(tpos) >= 0) //inside solid reaction
            {
                Point oldtpos = Tilemap.PosToTile(position - velocity * delta);
                Point inDir = tpos - oldtpos;
                if (portalSys.TileHasDisabledCollision(oldtpos, inDir))
                {
                    Teleport(portalSys.GetPortalFromTile(oldtpos, inDir));
                    tpos = Tilemap.PosToTile(position);
                    AfterTeleportLogic();
                }
                else
                {
                    velocity *= 1.1f;
                    return;
                }
            }

            foreach (Point tileOffset in axisAlignedOffsets)
            {
                Point collTile = tpos + tileOffset;
                if (tilemap.GetTileType(collTile) >= 0)
                {
                    if (portalSys.TileHasDisabledCollision(tpos, tileOffset) )
                    {
                        continue;
                    }
                    Vector2 tileOffsetVector = tileOffset.ToVector2();
                    Vector2 boundry = tileOffsetVector * tileSize * 0.5f;
                    Vector2 localPos = Mathlike.WrapV(position, new Vector2(tileSize, tileSize)) - new Vector2(tileSize / 2f, tileSize / 2f);
                    Vector2 entityBoundry = localPos + tileOffsetVector * collisionradious;
                    //float overlap = Mathlike.ProjectionFactor(entityBoundry, boundry) - 1;
                    float overlap = Vector2.Dot(entityBoundry,boundry) / (tileSize/2) - (tileSize/2);
                    if (overlap > 0)
                    {
                        CollisionReactionAxisAligned(tpos, -tileOffsetVector, overlap);
                    }
                }
            }

            foreach (Point tileOffset in diagonalOffsets)
            {
                Point collTile = tpos + tileOffset;
                if (tilemap.GetTileType(collTile) >= 0)
                {
                    Vector2 tileOffsetVector = tileOffset.ToVector2();
                    Vector2 corner = tileOffsetVector * tileSize * 0.5f;
                    Vector2 localPos = Mathlike.WrapV(position, new Vector2(tileSize, tileSize)) - new Vector2(tileSize / 2f, tileSize / 2f);
                    Vector2 normalAndDistance = localPos - corner;
                    float overlap = collisionradious - normalAndDistance.Length();
                    if (overlap > 0)
                    {
                        normalAndDistance.Normalize();
                        CollisionReactionDiagonal(tpos, normalAndDistance, overlap);
                        
                    }
                }
            }

            //----------------------------------------------------------------
            //------------byter ut typ allt under mot foreach ovan------------
            //----------------------------------------------------------------
            /*
             
            #region axis aligned collision detection


            if (tilemap.GetTileType(new Point(tpos.X, tpos.Y + 1)) >= 0) //down
            {
                bool noPortalHinderingColl = true;
                if(portalSys.bluePortalsExists && portalSys.bluePortalFrame.coord == tpos && portalSys.bluePortalFrame.side == 0)
                {
                    noPortalHinderingColl = false;
                }
                else if (portalSys.yellowPortalsExists && portalSys.yellowPortalFrame.coord == tpos && portalSys.yellowPortalFrame.side == 0)
                {
                    noPortalHinderingColl = false;
                }
                if (noPortalHinderingColl)
                {
                    float boundry = (tpos.Y + 1) * tileSize - collisionradious;
                    if (position.Y > boundry)
                    {
                        position.Y = boundry;
                        if (velocity.Y < 150.0f) //nått mojs idk
                        {
                            ReactVelocityExtraParam(new Vector2(0, -1), 0, -1); //velocity.Y = MathF.Min(velocity.Y, -velocity.Y*0.6f);
                        }
                        else
                        {
                            ReactVelocity(new Vector2(0, -1));
                        }
                    }
                }
                
            }
            if (tilemap.GetTileType(new Point(tpos.X - 1, tpos.Y)) >= 0) //left
            {
                float boundry = (tpos.X + 0) * tileSize + collisionradious;
                if (position.X < boundry)
                {
                    position.X = boundry;
                    ReactVelocity(new Vector2(1, 0));  //velocity.X = MathF.Max(velocity.X, tempBounceSpeed);

                }
            }
            if (tilemap.GetTileType(new Point(tpos.X, tpos.Y - 1)) >= 0) //up
            {
                float boundry = (tpos.Y + 0) * tileSize + collisionradious;
                if (position.Y < boundry)
                {
                    position.Y = boundry;
                    ReactVelocity(new Vector2(0, 1));  //velocity.Y = MathF.Max(velocity.Y, tempBounceSpeed);
                }
            }
            if (tilemap.GetTileType(new Point(tpos.X + 1, tpos.Y)) >= 0) //right
            {
                float boundry = (tpos.X + 1) * tileSize - collisionradious;
                if (position.X > boundry)
                {
                    position.X = boundry;
                    ReactVelocity(new Vector2(-1, 0));  //velocity.X = MathF.Min(velocity.X, -tempBounceSpeed);
                }
            }

            #endregion
            #region corner collision detection

            if (colltype == CollisionShapeType.circle)
            {
                //AI
                // Diagonal Collisions (Top-Left, Top-Right, Bottom-Left, Bottom-Right)
                Point[] diagonals = { new Point(-1, -1), new Point(1, -1), new Point(-1, 1), new Point(1, 1) };

                foreach (var dir in diagonals)
                {
                    if (tilemap.GetTileType(new Point(tpos.X + dir.X, tpos.Y + dir.Y)) >= 0)
                    {
                        // Calculate the specific corner of the tile we might be hitting
                        Vector2 corner = new Vector2(
                            (tpos.X + (dir.X < 0 ? 0 : 1)) * tileSize,
                            (tpos.Y + (dir.Y < 0 ? 0 : 1)) * tileSize
                        );

                        Vector2 diff = position - corner;
                        if (diff.Length() < collisionradious)
                        {
                            // Push out based on the normal from the corner
                            Vector2 normal = Vector2.Normalize(diff);
                            position = corner + normal * collisionradious;

                            ReactVelocity(normal);
                            // Reflect/Dampen velocity along the collision normal
                            //float dot = Vector2.Dot(velocity, normal);
                            //if (dot < 0)
                            //{
                            //    velocity -= normal * dot * 1.6f; // 0.6 bounce factor matching your axis logic
                            //}
                        }
                    }
                }

            }
            else if (colltype == CollisionShapeType.square)
            {
                // Define the 4 corners: (Tile Offset, Corner Multiplier, Normal Direction)
                var corners = new[]
                {
                 new { Offset = new Point(-1, -1), Corner = new Vector2(0, 0), Norm = new Vector2(1, 1) },  // Top-Left
                 new { Offset = new Point(1, -1),  Corner = new Vector2(1, 0), Norm = new Vector2(-1, 1) }, // Top-Right
                 new { Offset = new Point(-1, 1),  Corner = new Vector2(0, 1), Norm = new Vector2(1, -1) }, // Bottom-Left
                 new { Offset = new Point(1, 1),   Corner = new Vector2(1, 1), Norm = new Vector2(-1, -1) } // Bottom-Right
                };

                foreach (var c in corners)
                {
                    // 1. Check if diagonal is solid AND neighbors are empty (to prevent snagging on walls)
                    bool diagonalSolid = tilemap.GetTileType(new Point(tpos.X + c.Offset.X, tpos.Y + c.Offset.Y)) >= 0;
                    bool sideXEmpty = tilemap.GetTileType(new Point(tpos.X + c.Offset.X, tpos.Y)) < 0;
                    bool sideYEmpty = tilemap.GetTileType(new Point(tpos.X, tpos.Y + c.Offset.Y)) < 0;

                    if (diagonalSolid && sideXEmpty && sideYEmpty)
                    {
                        // 2. Calculate the specific corner point
                        Vector2 cornerPos = new Vector2((tpos.X + c.Corner.X) * tileSize, (tpos.Y + c.Corner.Y) * tileSize);

                        // 3. Calculate penetration (how deep the player square is past the corner)
                        float distX = (c.Offset.X < 0) ? (cornerPos.X + collisionradious) - position.X : position.X - (cornerPos.X - collisionradious);
                        float distY = (c.Offset.Y < 0) ? (cornerPos.Y + collisionradious) - position.Y : position.Y - (cornerPos.Y - collisionradious);

                        // 4. If both are positive, we are overlapping the corner
                        if (distX > 0 && distY > 0)
                        {
                            // Resolve along the shallower axis
                            if (distX < distY)
                            {
                                position.X = (c.Offset.X < 0) ? cornerPos.X + collisionradious : cornerPos.X - collisionradious;
                                ReactVelocity(new Vector2(c.Norm.X, 0)); // Horizontal push
                            }
                            else
                            {
                                position.Y = (c.Offset.Y < 0) ? cornerPos.Y + collisionradious : cornerPos.Y - collisionradious;
                                ReactVelocity(new Vector2(0, c.Norm.Y)); // Vertical push
                            }
                        }
                    }
                }

            }
            //else används inte, behövs inte
            //{  
            //    //AI
            //    // Square Bounding Box offsets
            //    float left = position.X - collisionradious;
            //    float right = position.X + collisionradious;
            //    float top = position.Y - collisionradious;
            //    float bottom = position.Y + collisionradious;

            //    Point[] diagonals = { new Point(-1, -1), new Point(1, -1), new Point(-1, 1), new Point(1, 1) };

            //    foreach (var dir in diagonals)
            //    {
            //        // Only check diagonal if the two adjacent cardinal tiles are empty
            //        // This prevents jittering when sliding along a flat wall of tiles
            //        bool side1Solid = tilemap.GetTileType(new Point(tpos.X + dir.X, tpos.Y)) >= 0;
            //        bool side2Solid = tilemap.GetTileType(new Point(tpos.X, tpos.Y + dir.Y)) >= 0;

            //        if (!side1Solid && !side2Solid && tilemap.GetTileType(new Point(tpos.X + dir.X, tpos.Y + dir.Y)) >= 0)
            //        {
            //            float cornerX = (dir.X < 0 ? tpos.X : tpos.X + 1) * tileSize;
            //            float cornerY = (dir.Y < 0 ? tpos.Y : tpos.Y + 1) * tileSize;

            //            // Check if the square bounding box contains the corner point
            //            bool overlapX = (dir.X < 0) ? (left < cornerX) : (right > cornerX);
            //            bool overlapY = (dir.Y < 0) ? (top < cornerY) : (bottom > cornerY);

            //            if (overlapX && overlapY)
            //            {
            //                // Resolve along the shallower axis (Standard AABB resolution)
            //                float diffX = Math.Abs(position.X - cornerX);
            //                float diffY = Math.Abs(position.Y - cornerY);

            //                if (collisionradious - diffX < collisionradious - diffY)
            //                {
            //                    position.X = cornerX + (dir.X < 0 ? collisionradious : -collisionradious);
            //                    velocity.X = Math.Min(velocity.X, -velocity.X * 0.6f);
            //                }
            //                else
            //                {
            //                    position.Y = cornerY + (dir.Y < 0 ? collisionradious : -collisionradious);
            //                    velocity.Y = Math.Min(velocity.Y, -velocity.Y * 0.6f);
            //                }
            //            }
            //        }
            //    }
            //}

            #endregion

            */
        }

        protected virtual void CollisionReactionAxisAligned(Point tile, Vector2 normal, float overlap)
        {
            ReactVelocity(normal);
            ReactPosition(normal, overlap);
        }
        protected virtual void CollisionReactionDiagonal(Point tile, Vector2 normal, float overlap)
        {
            ReactVelocity(normal);
            ReactPosition(normal, overlap);
        }

        private void ReactVelocity(Vector2 normal)
        {
            float dot = Vector2.Dot(velocity, normal);
            if (dot > 0) return;
            Vector2 velNorm = normal * -dot * bounciness;
            Vector2 velPara = (velocity - normal * dot) * slidyness;
            velocity = velNorm + velPara;

        }
        private void ReactVelocityExtraParam(Vector2 normal, float bounce2, float slidy2)
        {
            if(bounce2 <0) { bounce2 = bounciness; }
            if (slidy2 < 0) { slidy2 = slidyness; }
            float dot = Vector2.Dot(velocity, normal);
            if (dot > 0) return;
            Vector2 velNorm = normal * -dot * bounce2;
            Vector2 velPara = (velocity - normal * dot) * slidy2;
            velocity = velNorm + velPara;

        }
        //private void ReactVelocityExtraParam(Vector2 normal, float bounce2, float slidy2)
        //{
        //    if (bounce2 < 0) { bounce2 = bounciness; }
        //    if (slidy2 < 0) { slidy2 = slidyness; }
        //    float dot = Vector2.Dot(velocity, normal);
        //    if (dot > 0) return;
        //    Vector2 velNorm = normal * -dot * (bounciness / 3 + bounce2 * 2 / 3);
        //    Vector2 velPara = (velocity - normal * dot) * (slidyness / 3 + slidy2 * 2 / 3);
        //    velocity = velNorm + velPara;

        //}

        private void ReactPosition(Vector2 normal, float overlap)
        {
            position += normal * overlap;
        }

        public void Teleport(Portal entryPortal)
        {
            Portal destPortal = portalSys.GetLinkedPortal(entryPortal);
            bool doAFlip = entryPortal.flipped != destPortal.flipped;

            Vector2 localPos = Mathlike.WrapV(position, new Vector2(tileSize, tileSize)) - new Vector2(tileSize / 2f, tileSize / 2f);
            Point inDir = entryPortal.inDirection;
            Point outDir = Point.Zero - destPortal.inDirection;

            float angleIn = MathF.Atan2(inDir.Y , inDir.X); //X Y was flipped on both
            float angleOut = MathF.Atan2(outDir.Y, outDir.X);
            float rotation = angleOut - angleIn;
            localPos.Rotate(rotation);
            if (doAFlip) { localPos = Vector2.Reflect(localPos, Vector2.Rotate(outDir.ToVector2(),MathF.PI/2) ); }
            Vector2 destPos = Tilemap.TileTOPosCenter(destPortal.tile) + localPos;

            velocity.Rotate(rotation);
            if (doAFlip) { velocity = Vector2.Reflect(velocity, Vector2.Rotate(outDir.ToVector2(), MathF.PI / 2)); }
            position = destPos;

        }

        protected virtual void ClampVelocity(float delta)
        {
            //velocity = new Vector2(MathF.Min(MathF.Max(velocity.X, -collisionradious / delta), collisionradious / delta), MathF.Min(MathF.Max(velocity.Y, -collisionradious / delta), collisionradious / delta));
            float maxSpeed = collisionradious / delta;
            velocity.X = Mathlike.ClampF(velocity.X, -maxSpeed, maxSpeed);
            velocity.Y = Mathlike.ClampF(velocity.Y, -maxSpeed, maxSpeed);

        }

        protected virtual void AfterTeleportLogic() { }


    }
}
