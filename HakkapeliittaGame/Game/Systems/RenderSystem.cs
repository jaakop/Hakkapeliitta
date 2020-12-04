using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MGPhysics;
using ReeGame.Components;

namespace ReeGame.Systems
{
    public static class RenderSystem
    {
        /// <summary>
        /// Draws sprites on to screen
        /// </summary>
        /// <param name="sprites">Dictionary of the drawabel sprites</param>
        /// <param name="transfroms">Dictionary of transfroms</param>
        /// <param name="spriteBatch">SpriteBatch to draw the sprites with</param>
        public static void RenderSprites(Dictionary<Entity, Sprite> sprites, Dictionary<Entity, Transform> transfroms, SpriteBatch spriteBatch)
        {
            foreach(KeyValuePair<Entity, Sprite> sprite in sprites)
            {
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
