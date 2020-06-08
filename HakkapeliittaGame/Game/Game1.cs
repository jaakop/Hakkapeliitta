using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MGPhysics;
using MGPhysics.Components;
using MGPhysics.Systems;

using ReeGame.Components;

namespace ReeGame
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Dictionary<Entity, Transform> transfroms;
        Dictionary<Entity, RigidBody> rigidBodies;
        Dictionary<Entity, Sprite> sprites;

        Dictionary<Entity, GroupComponent> groups;

        int movementSpeed;
        bool mousePressed;

        Entity palikka1;
        Entity targetPalikka;
        Entity group;

        Camera2D camera;

        public Game1()
        {

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Entity.InitializeKeyIndex();

            IsMouseVisible = true;
            movementSpeed = 7;
            mousePressed = false;

            camera = new Camera2D(new Vector(0, 0), 0.5f);

            transfroms = new Dictionary<Entity, Transform>();
            rigidBodies = new Dictionary<Entity, RigidBody>();
            sprites = new Dictionary<Entity, Sprite>();

            groups = new Dictionary<Entity, GroupComponent>();

            targetPalikka = Entity.NewEntity();
            CreateSprite(targetPalikka, BasicTexture(Color.HotPink), Color.White);
            
            palikka1 = Entity.NewEntity();
            CreatePalikka(palikka1, new Vector(0, 0), new Vector(100, 100));

            group = CreateNewGroup(palikka1);

            for(int i = 0; i < 10; i++)
            {
                Entity palikka = Entity.NewEntity();
                CreatePalikka(palikka, new Vector(0, 100 + 100 * i), new Vector(75, 75));
                AddMemberToGroup(palikka, group);
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (mouseState.LeftButton == ButtonState.Pressed && !mousePressed)
            {
                if (GraphicsDevice.Viewport.Bounds.Contains(mouseState.Position))
                {
                    Vector mousePosition = new Vector(camera.Position.X + mouseState.Position.X / camera.Zoom - GraphicsDevice.Viewport.Width,
                                                        camera.Position.Y + mouseState.Position.Y / camera.Zoom - GraphicsDevice.Viewport.Height);
                    CreateTransform(targetPalikka, mousePosition, new Vector(25, 25));
                    mousePressed = true;
                }
            }
            else if(mouseState.LeftButton == ButtonState.Released)
            {
                mousePressed = false;   
            }

            Vector velocity = new Vector(0,0);
            /*
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                velocity += new Vector(0, -1 * movementSpeed);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                velocity += new Vector(0, 1 * movementSpeed);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                velocity += new Vector(-1 * movementSpeed,0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                velocity += new Vector(1 * movementSpeed, 0);
            }
            */

            if(transfroms.ContainsKey(targetPalikka))
                velocity = Vector.Lerp(transfroms[palikka1].Position, transfroms[targetPalikka].Position, 0.1f) * movementSpeed;

            if (Math.Abs(velocity.X) > movementSpeed)
                velocity.X = (velocity.X / Math.Abs(velocity.X)) * movementSpeed;
            if (Math.Abs(velocity.Y) > movementSpeed)
                velocity.Y = (velocity.Y / Math.Abs(velocity.Y)) * movementSpeed;

            PhysicsSystem.MoveEntity(palikka1, velocity, ref transfroms, rigidBodies);

            foreach(Entity member in groups[group].Members)
            {
                PhysicsSystem.MoveEntity(member, new Vector(1, 0), ref transfroms, rigidBodies);
            }

            camera.Position = transfroms[palikka1].Position;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.GetTransformationMatrix(GraphicsDevice.Viewport));
            RenderSystem.RenderSprites(sprites, transfroms, spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        
        /// <summary>
        /// Creates basic palikka with rigidbody
        /// </summary>
        /// <param name="palikka">Palikka Entity</param>
        /// <param name="position">Position to set the palikka</param>
        /// <param name="size">Size of the palikka</param>
        void CreatePalikka(Entity palikka, Vector position, Vector size)
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
        void CreateSprite(Entity entity, Texture2D texture, Color color)
        {

            if (!sprites.ContainsKey(entity))
            {
                sprites.Add(entity, new Sprite(texture, color));
            }
            else
            {
                sprites[entity] = new Sprite(texture, color);
            }

        }

        /// <summary>
        /// Creates transform compomnent
        /// </summary>
        /// <param name="entity">Transforms Entity</param>
        /// <param name="position">Position</param>
        /// <param name="size">Size</param>
        void CreateTransform(Entity entity, Vector position, Vector size)
        {
            if (!transfroms.ContainsKey(entity))
            {
                transfroms.Add(entity, new Transform(position, size));
            }
            else
            {
                transfroms[entity] = new Transform(position, size);
            }
        }

        /// <summary>
        /// Creates rigidBody component
        /// </summary>
        /// <param name="entity">Component entity</param>
        /// <param name="size">size of the hitbox</param>
        void CreateRigidBody(Entity entity, Vector size)
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
        Texture2D BasicTexture(Color color)
        {
            Texture2D basicTexture = new Texture2D(GraphicsDevice, 1, 1);
            basicTexture.SetData(new Color[] { color });

            return basicTexture;
        }

        /// <summary>
        /// Creates a new group
        /// </summary>
        /// <param name="leaderEntity"></param>
        Entity CreateNewGroup(Entity leaderEntity)
        {
            foreach(KeyValuePair<Entity, GroupComponent> group in groups)
            {
                if (group.Value.LeaderEntity == leaderEntity)
                    throw new Exception("Cannot assing leader entity. Entity is leaderEntity of a another group");

                group.Value.RemoveMember(leaderEntity);
            }
            Entity groupEntity = Entity.NewEntity();
            groups.Add(groupEntity, new GroupComponent(leaderEntity));
            return groupEntity;
        }

        /// <summary>
        /// Adds an entity to a group
        /// </summary>
        /// <param name="member"></param>
        /// <param name="group"></param>
        void AddMemberToGroup(Entity member, Entity group)
        {
            foreach(KeyValuePair<Entity, GroupComponent> checkGroup in groups)
            {
                if(checkGroup.Value.LeaderEntity == member)
                    throw new Exception("Cannot assing member entity. Entity is leaderEntity of a another group");

                checkGroup.Value.RemoveMember(member);
            }
            groups[group].Members.Add(member);
        }
    }
}
