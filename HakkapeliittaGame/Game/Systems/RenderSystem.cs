using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MGPhysics;
using ReeGame.Components;

namespace ReeGame.Systems
{
    public class RenderSystem : ISystem
    {
        private SpriteBatch spriteBatch;

        public RenderSystem(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }

        public void Call(ECSManager manager, int deltaTime)
        {
            foreach(KeyValuePair<Entity, Sprite> sprite in manager.ComponentManager.GetComponentArray<Sprite>().Array)
            {
                Dictionary<Entity, Transform> transfroms = manager.ComponentManager.GetComponentArray<Transform>().Array;

                if (!transfroms.ContainsKey(sprite.Key))
                    continue;

                Transform transform = transfroms[sprite.Key];

                spriteBatch.Draw(sprite.Value.Texture, 
                                 new Rectangle(transform.Position.IntegerX - transform.Size.IntegerX/2, transform.Position.IntegerY - transform.Size.IntegerY / 2, 
                                                transform.Size.IntegerX, transform.Size.IntegerY), 
                                 sprite.Value.ColorMask);
            }

        }
    }
}
