using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MGPhysics;
using MGPhysics.Components;
using MGPhysics.Systems;

using ReeGame.Components;
using ReeGame.Systems;

namespace ReeGame.Systems
{
    public static class GameSystem
    {
        /// <summary>
        /// Creates basic palikka with rigidbody
        /// </summary>
        /// <param name="palikka">Palikka Entity</param>
        /// <param name="position">Position to set the palikka</param>
        /// <param name="size">Size of the palikka</param>
        public static void CreatePalikka(Entity palikka, Vector position, Vector size)
        {
            //Add sprite
            CreateSprite(palikka, BasicTexture(Color.White), Color.White);

            //Add transform
            CreateTransform(palikka, position, size);

            //Add rigidbody
            CreateRigidBody(palikka, size);
        }

        /// <summary>
        /// Creates a sprite component
        /// </summary>
        /// <param name="entity">Sprites entity</param>
        /// <param name="texture">texture of the sprite</param>
        /// <param name="color">Color mask</param>
        public static Dictionary<Entity, Sprite> CreateSprite(Entity entity, Texture2D texture, Color color, Dictionary<Entity, Sprite> sprites)
        {

            if (!sprites.ContainsKey(entity))
            {
                sprites.Add(entity, new Sprite(texture, color));
            }
            else
            {
                sprites[entity] = new Sprite(texture, color);
            }
            return sprites;
        }

        /// <summary>
        /// Creates transform compomnent
        /// </summary>
        /// <param name="entity">Transforms Entity</param>
        /// <param name="position">Position</param>
        /// <param name="size">Size</param>
        public static void CreateTransform(Entity entity, Vector position, Vector size)
        {
            if (!transforms.ContainsKey(entity))
            {
                transforms.Add(entity, new Transform(position, size));
            }
            else
            {
                transforms[entity] = new Transform(position, size);
            }
        }

        /// <summary>
        /// Creates rigidBody component
        /// </summary>
        /// <param name="entity">Component entity</param>
        /// <param name="size">size of the hitbox</param>
        public static void CreateRigidBody(Entity entity, Vector size)
        {
            if (!rigidBodies.ContainsKey(entity))
            {
                rigidBodies.Add(entity, new RigidBody(size));
            }
            else
            {
                rigidBodies[entity] = new RigidBody(size);
            }
        }

        /// <summary>
        /// Creates a basic box texture
        /// </summary>
        /// <param name="color">Color of the box</param>
        /// <returns></returns>
        public static Texture2D BasicTexture(Color color)
        {
            Texture2D basicTexture = new Texture2D(GraphicsDevice, 1, 1);
            basicTexture.SetData(new Color[] { color });

            return basicTexture;
        }
    }
}
