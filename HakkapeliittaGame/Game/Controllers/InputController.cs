using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MGPhysics;

using ReeGame.Components;
using ReeGame.Systems;

namespace ReeGame.Controllers
{
    class InputController
    {
        private List<KeyMapping> keyMappings;

        public bool leftMouseButtonDown;
        public bool middleMouseButtonDown;
        public bool rightMouseButtonDown;

        /// <summary>
        /// Input controller
        /// </summary>
        public InputController()
        {
            leftMouseButtonDown = false;
            middleMouseButtonDown = false;
            rightMouseButtonDown = false;
            keyMappings = new List<KeyMapping>();
        }

        /// <summary>
        /// Check all input mappings including mouse buttons left middle & right
        /// </summary>
        public void CheckInput()
        {
            foreach(KeyMapping keyMapping in keyMappings)
            {
                keyMapping.Check();
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                LeftMouseButtonMapping?.Invoke();
                leftMouseButtonDown = true;
            }
            else leftMouseButtonDown = false;

            if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
            {
                MiddleMouseButtonMapping?.Invoke();
                middleMouseButtonDown = true;
            }
            else middleMouseButtonDown = false;

            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                RightMouseButtonMapping?.Invoke();
                rightMouseButtonDown = true;
            }
            else rightMouseButtonDown = false;
        }

        /// <summary>
        /// Add a new key mapping to the mappings
        /// </summary>
        /// <param name="mapping">The new mapping to add</param>
        public void AddKeyMapping(KeyMapping mapping)
        {
            keyMappings.Add(mapping);
        }

        public Action LeftMouseButtonMapping { get; set; }
        public Action MiddleMouseButtonMapping { get;  set; }
        public Action RightMouseButtonMapping { get; set; }


    }
}
