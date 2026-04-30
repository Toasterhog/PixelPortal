using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGSimpelFysik
{
    public class Physics
    {
        public List<PhysicalEntity> entities = new List<PhysicalEntity>();
        public int worldWidth = 500;
        public int worldHeight = 500;
        public List<PhysicalEntity> pEntitiesToAdd = new List<PhysicalEntity>();
        public List<PhysicalEntity> pEntitiesToRemove = new List<PhysicalEntity>();
        private float deltaMultiplier = 0.7f;
        public Physics(int worldWidth, int worldHeight)
        {
            this.worldWidth = worldWidth;
            this.worldHeight = worldHeight;
        }

        public void AddEntity(PhysicalEntity entity)
        {
            pEntitiesToAdd.Add(entity);
        }
        public void RemoveEntity(PhysicalEntity entity) 
        {
            pEntitiesToRemove.Add(entity); 
        }

        public void Update(GameTime gameTime)
        {
            foreach (PhysicalEntity PE in pEntitiesToAdd) //modifiera entities listan innan istället för medans foreach körs = inte error
            {
                entities.Add(PE);
            }
            pEntitiesToAdd.Clear();
            foreach (PhysicalEntity PE in pEntitiesToRemove)
            {
                entities.Remove(PE);
            }
            pEntitiesToRemove.Clear();

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds * deltaMultiplier;
            foreach (PhysicalEntity entity in entities)
            {
                entity.PhysicsUpdate(delta);

             
                float posX = entity.position.X; //wrapa
                if (posX > worldWidth)
                {
                    entity.position.X = posX % worldWidth;
                }
                else if (posX < 0)
                {
                    entity.position.X = (posX + 5 * worldWidth) % worldWidth; // 5 = godtyckligt, räcker med 1
                }
                float posY = entity.position.Y;
                if (posY > worldHeight)
                {
                    entity.position.Y = posY % worldHeight;
                }
                else if (posY < 0)
                {
                    entity.position.Y = (posY + 5 * worldHeight) % worldHeight;
                }
            }
        }
    }
}
