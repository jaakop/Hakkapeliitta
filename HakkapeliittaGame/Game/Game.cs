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
        PlayerController playerController;

        int movementSpeed;
        bool boosted;

        Entity palikka1;
        Entity targetPalikka;

        Camera2D camera;
        Random rnd;

        Effect testEffect;

        public Game()
        {

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Common.GraphicsDevice = GraphicsDevice;
            Common.RND = new Random();
            manager = new ECSManager();
            inputController = new InputController();
            playerController = new PlayerController(manager, new ArmyController(manager));

            rnd = new Random();

            IsMouseVisible = true;
            movementSpeed = 7;

            camera = new Camera2D(new Vector(0, -100), 1f);

            manager.ComponentManager.RegisterComponent<Transform>();
            manager.ComponentManager.RegisterComponent<RigidBody>();
            manager.ComponentManager.RegisterComponent<Sprite>();
            manager.ComponentManager.RegisterComponent<GroupComponent>();
            manager.ComponentManager.RegisterComponent<MovementComponent>();

            targetPalikka = manager.EntityManager.CreateEntity();
            Common.CreateSprite(manager, targetPalikka, Common.BasicTexture(Color.HotPink), Color.White);

            playerController.ArmyController.AddNewUnitGroup(16, new Vector(100, 100), movementSpeed, 5, 7, 150f);
            playerController.ArmyController.AddNewUnitGroup(16, new Vector(100, 100), movementSpeed, 5, 7, 150f);
            playerController.ArmyController.AddNewUnitGroup(16, new Vector(100, 100), movementSpeed, 5, 7, 150f);

            playerController.SelectGroup(0);

            palikka1 = manager.ComponentManager.GetComponent<GroupComponent>(playerController.SelectedGroup).LeaderEntity;

            playerController.ArmyController.MoveGroup(playerController.SelectedGroup, manager.ComponentManager.GetComponent<Transform>(palikka1).Position);
            playerController.ArmyController.MoveGroup(playerController.ArmyController.GetGroups()[1], new Vector(-1000, 0));
            playerController.ArmyController.MoveGroup(playerController.ArmyController.GetGroups()[2], new Vector(1000, 0));

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
            inputController.AddKeyMapping(new KeyMapping(() =>
            {
                Entity member = manager.EntityManager.CreateEntity();
                var groupComponent = playerController.SelectedGroupComponent;
                groupComponent.Members.Add(member);
                Common.CreatePalikka(manager, member, manager.ComponentManager.GetComponent<Transform>(playerController.SelectedGroupComponent.LeaderEntity).Position, new Vector(100,100));
                Common.CreateMovement(manager, member, new Vector(0, 0), movementSpeed + Common.RND.Next(0, 5));

                playerController.SelectedGroupComponent = groupComponent;
                playerController.ArmyController.MoveGroup(playerController.SelectedGroup, manager.ComponentManager.GetComponent<Transform>(groupComponent.LeaderEntity).Position);
            }, Keys.U, true));
            inputController.AddKeyMapping(new KeyMapping(() =>
            {
                var groupComponent = playerController.SelectedGroupComponent;
                if (groupComponent.Members.Count < 1)
                    return;

                manager.ComponentManager.EntityDestroyed(playerController.SelectedGroupComponent.Members[0]);
                groupComponent.Members.RemoveAt(0);

                playerController.SelectedGroupComponent = groupComponent;
                playerController.ArmyController.MoveGroup(playerController.SelectedGroup, manager.ComponentManager.GetComponent<Transform>(groupComponent.LeaderEntity).Position);
            }, Keys.I, true));
            inputController.AddKeyMapping(new KeyMapping(() =>
            {
                var groupComponent = playerController.SelectedGroupComponent;
                groupComponent.RowLenght++;
                playerController.SelectedGroupComponent = groupComponent;

                playerController.ArmyController.MoveGroup(playerController.SelectedGroup, manager.ComponentManager.GetComponent<Transform>(groupComponent.LeaderEntity).Position);
            }, Keys.R, true));
            inputController.AddKeyMapping(new KeyMapping(() =>
            {
                var groupComponent = playerController.SelectedGroupComponent;

                if (groupComponent.Direction - 1 > -360)
                    groupComponent.Direction -= 1;
                else
                    groupComponent.Direction = 0;

                playerController.SelectedGroupComponent = groupComponent;
                playerController.ArmyController.MoveGroup(playerController.SelectedGroup, manager.ComponentManager.GetComponent<Transform>(groupComponent.LeaderEntity).Position);
            }, Keys.A, false));
            inputController.AddKeyMapping(new KeyMapping(() =>
            {
                var groupComponent = playerController.SelectedGroupComponent;

                if (groupComponent.Direction + 1 < 360)
                    groupComponent.Direction += 1;
                else
                    groupComponent.Direction = 0;

                playerController.SelectedGroupComponent = groupComponent;
                playerController.ArmyController.MoveGroup(playerController.SelectedGroup, manager.ComponentManager.GetComponent<Transform>(groupComponent.LeaderEntity).Position);
            }, Keys.D, false));
            inputController.AddKeyMapping(new KeyMapping(() =>
            {
                var groupComponent = playerController.SelectedGroupComponent;
                groupComponent.RowVariance++;

                playerController.SelectedGroupComponent = groupComponent;
                playerController.ArmyController.MoveGroup(playerController.SelectedGroup, manager.ComponentManager.GetComponent<Transform>(groupComponent.LeaderEntity).Position);
            }, Keys.M, true));
            inputController.AddKeyMapping(new KeyMapping(() =>
            {
                var groupComponent = playerController.SelectedGroupComponent;
                if (groupComponent.RowVariance <= 0)
                    return;

                groupComponent.RowVariance--;

                playerController.SelectedGroupComponent = groupComponent;
                playerController.ArmyController.MoveGroup(playerController.SelectedGroup, manager.ComponentManager.GetComponent<Transform>(groupComponent.LeaderEntity).Position);
            }, Keys.N, true));
            inputController.AddKeyMapping(new KeyMapping(() =>
            {
                boosted = true;
            }, Keys.LeftShift, false));

            inputController.AddKeyMapping(new KeyMapping(() =>
            {
                playerController.SelectGroup(0);
            }, Keys.D1, true));
            inputController.AddKeyMapping(new KeyMapping(() =>
            {
                playerController.SelectGroup(1);
            }, Keys.D2, true));
            inputController.AddKeyMapping(new KeyMapping(() =>
            {
                playerController.SelectGroup(2);
            }, Keys.D3, true));

            inputController.LeftMouseButtonMapping = () =>
            {
                Point mousePos = Mouse.GetState().Position;
                if (GraphicsDevice.Viewport.Bounds.Contains(mousePos) && this.IsActive && !inputController.leftMouseButtonDown)
                {
                    Vector mousePosition = camera.Position + new Vector(mousePos.X * 2 - GraphicsDevice.Viewport.Width, mousePos.Y * 2 - GraphicsDevice.Viewport.Height) / camera.Zoom / 2;
                    Common.CreateTransform(manager, targetPalikka, mousePosition, new Vector(25, 25));
                    
                    playerController.ArmyController.MoveGroup(playerController.SelectedGroup, mousePosition);
                }
            };
            inputController.MouseScrollMapping = (float change) =>
            {
                camera.Zoom += change * 0.0005f;
                if (camera.Zoom > 1) camera.Zoom = 1;
                else if (camera.Zoom < 0) camera.Zoom = 0.05f;
            };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            testEffect = Content.Load<Effect>("TestShader");
            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            boosted = false;
            int deltaTime = gameTime.ElapsedGameTime.Milliseconds;
            inputController.CheckInput();


            // camera.Position = transforms[palikka1].Position
            manager.SystemManager.Update(manager, deltaTime * (boosted ? 5 : 1));
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix transformMatrix = camera.GetTransformationMatrix(GraphicsDevice.Viewport);

            testEffect.Parameters["WorldViewProjection"].SetValue(transformMatrix);

            // TODO: Add your drawing code here 
            spriteBatch.Begin(effect: testEffect, blendState: BlendState.NonPremultiplied);

            Common.CallOneTimeSystems(manager, new RenderSystem(spriteBatch), gameTime.ElapsedGameTime.Milliseconds);

            spriteBatch.End();

            base.Draw(gameTime);
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
    }

    public static class Common
    {
        public static GraphicsDevice GraphicsDevice { get; set; }
        public static Random RND { get; set; }

        public static void CallOneTimeSystems(ECSManager manager, ISystem system, int deltaTime = 0)
        {
            system.Call(manager, deltaTime);
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

        /// <summary>
        /// Creates basic palikka with rigidbody
        /// </summary>
        /// <param name="palikka">Palikka Entity</param>
        /// <param name="position">Position to set the palikka</param>
        /// <param name="size">Size of the palikka</param>
        public static void CreatePalikka(ECSManager manager, Entity palikka, Vector position, Vector size)
        {
            //Add sprite
            CreateSprite(manager, palikka, BasicTexture(Color.White), Color.White);

            //Add transform
            CreateTransform(manager, palikka, position, size);

            //Add rigidbody
            CreateRigidBody(manager, palikka, size);

            //Add Movement component
            CreateMovement(manager, palikka, position, 100);
        }

        /// <summary>
        /// Creates a sprite component
        /// </summary>
        /// <param name="entity">Sprites entity</param>
        /// <param name="target">default target pos</param>
        /// <param name="speed">movement speed</param>
        public static void CreateMovement(ECSManager manager, Entity entity, Vector target, float speed)
        {
            MovementComponent mvc = new MovementComponent
            {
                target = target,
                velocity = speed
            };

            if (!manager.ComponentManager.GetComponentArray<MovementComponent>().Array.ContainsKey(entity))
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
        public static void CreateSprite(ECSManager manager, Entity entity, Texture2D texture, Color color)
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
        public static void CreateTransform(ECSManager manager, Entity entity, Vector position, Vector size)
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
        public static void CreateRigidBody(ECSManager manager, Entity entity, Vector size)
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
    }
}
