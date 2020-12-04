using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MGPhysics;

using ReeGame.Components;
using ReeGame.Systems;

namespace ReeGame
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ECSManager manager;

        int movementSpeed;

        Entity palikka1;
        Entity targetPalikka;
        Entity group;

        Camera2D camera;
        Random rnd;

        Dictionary<Entity, Vector> ToBeMoved;
        Dictionary<Entity, int> speedVariance;
        Dictionary<Keys, bool> pressedKeys;

        public Game()
        {

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            manager = new ECSManager();

            rnd = new Random();

            IsMouseVisible = true;
            movementSpeed = 7;

            camera = new Camera2D(new Vector(0, 0), 0.5f);

            manager.ComponentManager.RegisterComponent<Transform>();
            manager.ComponentManager.RegisterComponent<RigidBody>();
            manager.ComponentManager.RegisterComponent<Sprite>();
            manager.ComponentManager.RegisterComponent<GroupComponent>();

            ToBeMoved = new Dictionary<Entity, Vector>();
            speedVariance = new Dictionary<Entity, int>();
            pressedKeys = new Dictionary<Keys, bool>
            {

                //Keys from F20 onwards represent mouse buttons
                //F20 -> Left mouse button
                { Keys.F20, false },
                { Keys.F11, false }
            };

            targetPalikka = manager.EntityManager.CreateEntity();
            CreateSprite(targetPalikka, BasicTexture(Color.HotPink, GraphicsDevice), Color.White);

            palikka1 = manager.EntityManager.CreateEntity();
            CreatePalikka(palikka1, new Vector(0, 0), new Vector(100, 100));

            group = CreateNewGroup(palikka1);

            for (int i = 0; i < 15 - 1; i++)
            {
                Entity palikka = manager.EntityManager.CreateEntity();
                CreatePalikka(palikka, new Vector(0, 100 + 100 * i), new Vector(75, 75));
                AddMemberToGroup(palikka, group);
            }
            
            foreach (KeyValuePair<Entity, Vector> position in GroupSystem.CalculateGroupMemberPositions(
                manager.ComponentManager.GetComponentArray<Transform>().Array[palikka1].Position, 5, 150,
                manager.ComponentManager.GetComponentArray<GroupComponent>().Array[group]))
            {
                Transform transform = manager.ComponentManager.GetComponentArray<Transform>().Array[position.Key];
                transform.Position = position.Value;
                manager.ComponentManager.GetComponentArray<Transform>().Array[position.Key] = transform;
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
            CheckControls();

            //Loop through entities, that have a rigidbody and move them if they are ment to move
            foreach(KeyValuePair<Entity, RigidBody> entity in manager.ComponentManager.GetComponentArray<RigidBody>().Array)
            {
                MoveEntity(entity.Key);
            }

            // camera.Position = transforms[palikka1].Position

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.GetTransformationMatrix(GraphicsDevice.Viewport));
            RenderSystem.RenderSprites(manager.ComponentManager.GetComponentArray<Sprite>().Array, manager.ComponentManager.GetComponentArray<Transform>().Array, spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Checks controls
        /// </summary>
        private void CheckControls()
        {
            MouseState mouseState = Mouse.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F11) && !pressedKeys[Keys.F11])
            {
                if (!graphics.IsFullScreen)
                {
                    graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                }
                else
                {
                    graphics.PreferredBackBufferWidth = 800;
                    graphics.PreferredBackBufferHeight = 480;
                }
                graphics.ToggleFullScreen();
                pressedKeys[Keys.F11] = true;
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.F11))
            {
                pressedKeys[Keys.F11] = false;
            }

            if (mouseState.LeftButton == ButtonState.Pressed && !pressedKeys[Keys.F20])
            {
                if (GraphicsDevice.Viewport.Bounds.Contains(mouseState.Position))
                {
                    Vector mousePosition = new Vector(camera.Position.X + mouseState.Position.X / camera.Zoom - GraphicsDevice.Viewport.Width,
                                                        camera.Position.Y + mouseState.Position.Y / camera.Zoom - GraphicsDevice.Viewport.Height);
                    CreateTransform(targetPalikka, mousePosition, new Vector(25, 25));

                    MoveGroup(mousePosition, group);

                    pressedKeys[Keys.F20] = true;
                }
            }
            else if (mouseState.LeftButton == ButtonState.Released)
            {
                pressedKeys[Keys.F20] = false;
            }
        }

        /// <summary>
        /// Set destinations for a group
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="groupEntity"></param>
        private void MoveGroup(Vector mousePosition, Entity groupEntity)
        {
            //Move members
            foreach (KeyValuePair<Entity, Vector> position in GroupSystem.CalculateGroupMemberPositions(mousePosition, 5, 150,
                manager.ComponentManager.GetComponentArray<GroupComponent>().Array[groupEntity]))
            {
                int speedModifier = rnd.Next(-3, 3);
                if (ToBeMoved.ContainsKey(position.Key))
                    ToBeMoved[position.Key] = position.Value;
                else
                    ToBeMoved.Add(position.Key, position.Value);

                if (speedVariance.ContainsKey(position.Key))
                    speedVariance[position.Key] = speedModifier;
                else
                    speedVariance.Add(position.Key, speedModifier);
            }

            //Move the leader
            Entity leader = manager.ComponentManager.GetComponentArray<GroupComponent>().Array[groupEntity].LeaderEntity;
            int palikkaSpeedModifier = rnd.Next(-3, 3);

            if (ToBeMoved.ContainsKey(leader))
                ToBeMoved[leader] = mousePosition;
            else
                ToBeMoved.Add(leader, mousePosition);

            if (speedVariance.ContainsKey(leader))
                speedVariance[leader] = palikkaSpeedModifier;
            else
                speedVariance.Add(leader, palikkaSpeedModifier);
        }

        /// <summary>
        /// Moves entity
        /// </summary>
        /// <param name="member">Entity to be moved</param>
        private void MoveEntity(Entity member)
        {
            Vector velocity = new Vector(0, 0);

            if (!ToBeMoved.ContainsKey(member))
                return;

            velocity = Vector.Lerp(manager.ComponentManager.GetComponentArray<Transform>().Array[member].Position, ToBeMoved[member], 0.1f) * movementSpeed;

            if (Math.Abs(velocity.X) > movementSpeed)
                velocity.X = (velocity.X / Math.Abs(velocity.X)) * (movementSpeed + speedVariance[member]);
            if (Math.Abs(velocity.Y) > movementSpeed)
                velocity.Y = (velocity.Y / Math.Abs(velocity.Y)) * (movementSpeed + speedVariance[member]);

            manager.ComponentManager.GetComponentArray<Transform>().Array[member] = new Transform(manager.ComponentManager.GetComponentArray<Transform>().Array[member].Position + velocity, manager.ComponentManager.GetComponentArray<Transform>().Array[member].Size);

            if (manager.ComponentManager.GetComponentArray<Transform>().Array[member].Position == ToBeMoved[member])
                ToBeMoved.Remove(member);
            //PhysicsSystem.MoveEntity(member, velocity, ref transforms, rigidBodies);
        }

        /// <summary>
        /// Creates basic palikka with rigidbody
        /// </summary>
        /// <param name="palikka">Palikka Entity</param>
        /// <param name="position">Position to set the palikka</param>
        /// <param name="size">Size of the palikka</param>
        private void CreatePalikka(Entity palikka, Vector position, Vector size)
        {
            //Add sprite
            CreateSprite(palikka, BasicTexture(Color.White, GraphicsDevice), Color.White);

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
        private void CreateSprite(Entity entity, Texture2D texture, Color color)
        {

            if (!manager.ComponentManager.GetComponentArray<Sprite>().Array.ContainsKey(entity))
            {
                manager.ComponentManager.GetComponentArray<Sprite>().Array.Add(entity, new Sprite(texture, color));
            }
            else
            {
                manager.ComponentManager.GetComponentArray<Sprite>().Array[entity] = new Sprite(texture, color);
            }

        }

        /// <summary>
        /// Creates transform compomnent
        /// </summary>
        /// <param name="entity">Transforms Entity</param>
        /// <param name="position">Position</param>
        /// <param name="size">Size</param>
        private void CreateTransform(Entity entity, Vector position, Vector size)
        {
            if (!manager.ComponentManager.GetComponentArray<Transform>().Array.ContainsKey(entity))
            {
                manager.ComponentManager.GetComponentArray<Transform>().Array.Add(entity, new Transform(position, size));
            }
            else
            {
                manager.ComponentManager.GetComponentArray<Transform>().Array[entity] = new Transform(position, size);
            }
        }

        /// <summary>
        /// Creates rigidBody component
        /// </summary>
        /// <param name="entity">Component entity</param>
        /// <param name="size">size of the hitbox</param>
        private void CreateRigidBody(Entity entity, Vector size)
        {
            if (!manager.ComponentManager.GetComponentArray<RigidBody>().Array.ContainsKey(entity))
            {
                manager.ComponentManager.GetComponentArray<RigidBody>().Array.Add(entity, new RigidBody(size));
            }
            else
            {
                manager.ComponentManager.GetComponentArray<RigidBody>().Array[entity] = new RigidBody(size);
            }
        }

        /// <summary>
        /// Creates a basic box texture
        /// </summary>
        /// <param name="color">Color of the box</param>
        /// <returns></returns>
        public static Texture2D BasicTexture(Color color, GraphicsDevice graphics)
        {
            Texture2D basicTexture = new Texture2D(graphics, 1, 1);
            basicTexture.SetData(new Color[] { color });

            return basicTexture;
        }

        /// <summary>
        /// Creates a new group
        /// </summary>
        /// <param name="leaderEntity"></param>
        private Entity CreateNewGroup(Entity leaderEntity)
        {
            foreach (KeyValuePair<Entity, GroupComponent> group in manager.ComponentManager.GetComponentArray<GroupComponent>().Array)
            {
                if (group.Value.LeaderEntity == leaderEntity)
                    throw new Exception("Cannot assing leader entity. Entity is leaderEntity of a another group");

                GroupSystem.RemoveMember(leaderEntity, group.Value);
            }
            Entity groupEntity = manager.EntityManager.CreateEntity();
            manager.ComponentManager.GetComponentArray<GroupComponent>().Array.Add(groupEntity, new GroupComponent(leaderEntity));
            return groupEntity;
        }

        /// <summary>
        /// Adds an entity to a group
        /// </summary>
        /// <param name="member"></param>
        /// <param name="group"></param>
        private void AddMemberToGroup(Entity member, Entity group)
        {
            foreach (KeyValuePair<Entity, GroupComponent> checkGroup in manager.ComponentManager.GetComponentArray<GroupComponent>().Array)
            {
                if (checkGroup.Value.LeaderEntity == member)
                    throw new Exception("Cannot assing member entity. Entity is leaderEntity of a another group");

                GroupSystem.RemoveMember(member, checkGroup.Value);
            }
            manager.ComponentManager.GetComponentArray<GroupComponent>().Array[group].Members.Add(member);
        }
    }
}
