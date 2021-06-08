using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MGPhysics;

namespace ReeGame.Components
{
    public struct Sprite : IComponent
    {
        /// <summary>
        /// New sprite object
        /// </summary>
        /// <param name="texture">Texture of the sprite</param>
        /// <param name="color">Color mask of the sprite</param>
        public Sprite(Texture2D texture, Color color)
        {
            Texture = texture;
            ColorMask = color;
        }

        public Texture2D Texture{ get; }
        public Color ColorMask { get; }
    }
}
