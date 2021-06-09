using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MGPhysics;

using ReeGame.Components;
using ReeGame.Systems;
using ReeGame.Controllers;

namespace ReeGame
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ECSManager manager;
        InputController inputController;

        int movementSpeed;

        Entity palikka1;
        Entity targetPalikka;
        Entity group;

        Camera2D camera;
        Random rnd;

        Dictionary<Keys, bool> pressedKeys;

        public Game()
        {

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            manager = new ECSManager();
            inputController = new InputController();

            rnd = new Random();

            IsMouseVisible = true;
            movementSpeed = 7;

            camera = new Camera2D(new Vector(0, 0), 0.5f);

            manager.ComponentManager.RegisterComponent<Transform>();
            manager.ComponentManager.RegisterComponent<RigidBody>();
            manager.ComponentManager.RegisterComponent<Sprite>();
            manager.ComponentManager.RegisterComponent<GroupComponent>();
            manager.ComponentManager.RegisterComponent<MovementComponent>();

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

            MoveGroup(manager.ComponentManager.GetComponent<Transform>(palikka1).Position, group, 100, 10);

            Entity leader = manager.EntityManager.CreateEntity();
            Entity secondGroup = CreateNewGroup(leader);

            CreatePalikka(leader, new Vector(600,0), new Vector(100, 100));

            for (int i = 0; i < 20 - 1; i++)
            {
                Entity palikka = manager.EntityManager.CreateEntity();
                CreatePalikka(palikka, new Vector(0, 100 + 100 * i), new Vector(75, 75));
                CreateSprite(palikka, BasicTexture(Color.HotPink, GraphicsDevice), Color.White);
                AddMemberToGroup(palikka, secondGroup);
            }

            MoveGroup(manager.ComponentManager.GetComponent<Transform>(leader).Position, secondGroup, 100, 10);


            leader = manager.EntityManager.CreateEntity();
            secondGroup = CreateNewGroup(leader);

            CreatePalikka(leader, new Vector(-600, 0), new Vector(100, 100));

            for (int i = 0; i < 7 - 1; i++)
            {
                Entity palikka = manager.EntityManager.CreateEntity();
                CreatePalikka(palikka, new Vector(0, 100 + 100 * i), new Vector(75, 75));
                CreateSprite(palikka, BasicTexture(Color.RosyBrown, GraphicsDevice), Color.White);
                AddMemberToGroup(palikka, secondGroup);
            }

            MoveGroup(manager.ComponentManager.GetComponent<Transform>(leader).Position, secondGroup, 100, 10);


            manager.SystemManager.RegisterSystem(new MovementSystem());

            inputController.AddKeyMapping(new KeyMapping(() =>
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
            }, Keys.F11, true));

            inputController.AddKeyMapping(new KeyMapping(Exit, Keys.Escape));

            inputController.LeftMouseButtonMapping = () =>
            {
                Point mousePos = Mouse.GetState().Position;
                if (GraphicsDevice.Viewport.Bounds.Contains(mousePos) && !inputController.leftMouseButtonDown)
                {
                    Vector mousePosition = new Vector(camera.Position.X + mousePos.X / camera.Zoom - GraphicsDevice.Viewport.Width,
                                                        camera.Position.Y + mousePos.Y / camera.Zoom - GraphicsDevice.Viewport.Height);
                    CreateTransform(targetPalikka, mousePosition, new Vector(25, 25));

                    MoveGroup(mousePosition, group, movementSpeed, 3);
                }
            };

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
            int deltaTime = gameTime.ElapsedGameTime.Milliseconds;
            inputController.CheckInput();


            // camera.Position = transforms[palikka1].Position
            manager.SystemManager.Update(manager, deltaTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.GetTransformationMatrix(GraphicsDevice.Viewport));
            CallOneTimeSystems(new RenderSystem(spriteBatch), gameTime.ElapsedGameTime.Milliseconds);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Set destinations for a group
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="groupEntity"></param>
        /// <param name="speed">base speed</param>
        /// <param name="variance">Speed variance</param>
        private void MoveGroup(Vector mousePosition, Entity groupEntity, float speed, int variance)
        {
            Entity leader = manager.ComponentManager.GetComponent<GroupComponent>(groupEntity).LeaderEntity;

            //Move the leader
            MoveEntity(leader, mousePosition);
        
            //Move members
            CallOneTimeSystems(new GroupSystem(groupEntity, 6, 100), 0);

            Random rand = new Random();

            foreach(Entity member in manager.ComponentManager.GetComponent<GroupComponent>(groupEntity).Members)
            {
                MovementComponent mvc = manager.ComponentManager.GetComponent<MovementComponent>(member);
                mvc.velocity = speed + rand.Next(0, variance);
                manager.ComponentManager.UpdateComponent(member, mvc);
            }

            MovementComponent MvC = manager.ComponentManager.GetComponent<MovementComponent>(leader);
            MvC.velocity = speed;
            manager.ComponentManager.UpdateComponent(leader, MvC);
        }

        /// <summary>
        /// Moves entity
        /// </summary>
        /// <param name="member">Entity to be moved</param>
        private void MoveEntity(Entity member, Vector pos)
        {
            MovementComponent mvC = manager.ComponentManager.GetComponent<MovementComponent>(member);
            mvC.target = pos;

            manager.ComponentManager.UpdateComponent(member, mvC);
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

            //Add Movement component
            CreateMovement(palikka, position, 100);
        }

        /// <summary>
        /// Creates a sprite component
        /// </summary>
        /// <param name="entity">Sprites entity</param>
        /// <param name="target">default target pos</param>
        /// <param name="speed">movement speed</param>
        private void CreateMovement(Entity entity, Vector target, float speed)
        {
            MovementComponent mvc = new MovementComponent
            {
                target = target,
                velocity = speed
            };

            if (!manager.ComponentManager.GetComponentArray <MovementComponent>().Array.ContainsKey(entity))
            {
                manager.ComponentManager.GetComponentArray<MovementComponent>().Array.Add(entity, mvc);
            }
            else
            {
                manager.ComponentManager.UpdateComponent(entity, mvc);
            }

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
                manager.ComponentManager.UpdateComponent(entity, new Sprite(texture, color));
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
                manager.ComponentManager.UpdateComponent(entity, new Transform(position, size));
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
                manager.ComponentManager.UpdateComponent(entity, new RigidBody(size));
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
                if (!group.Value.ContainsEntity(leaderEntity)) continue;

                if (group.Value.LeaderEntity == leaderEntity)
                    throw new Exception("Cannot assing leader entity. Entity is leaderEntity of a another group");

                group.Value.Members.Remove(leaderEntity);
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
                if (!checkGroup.Value.ContainsEntity(member)) continue;
                if (checkGroup.Value.LeaderEntity == member)
                    throw new Exception("Cannot assing member entity. Entity is leaderEntity of a another group");

                return;
            }
            manager.ComponentManager.GetComponentArray<GroupComponent>().Array[group].Members.Add(member);
        }

        public void CallOneTimeSystems(ISystem system, int deltaTime)
        {
            system.Call(manager, deltaTime);
        }
    }
}
