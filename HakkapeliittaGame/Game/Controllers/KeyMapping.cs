using System;

using Microsoft.Xna.Framework.Input;

namespace ReeGame.Controllers
{
    class KeyMapping
    {
        public Keys key;
        private Action action;
        private bool pressed;

        private bool currentlyDown;

        /// <summary>
        /// Key mapping
        /// </summary>
        /// <param name="action">Action when key is down or pressed</param>
        /// <param name="key">Key, to which the action in linked to</param>
        /// <param name="pressed">If action is triggered only on press(true) or when key is down(default: false)</param>
        public KeyMapping(Action action, Keys key, bool pressed = false)
        {
            this.action = action;
            this.key = key;
            this.pressed = pressed;

            currentlyDown = false;
        }

        /// <summary>
        /// Check if mapped key is down
        /// </summary>
        public void Check()
        {
            if(Keyboard.GetState().IsKeyDown(key))
            {
                if (pressed && currentlyDown) return;

                action();
                currentlyDown = true;
            }
            if (Keyboard.GetState().IsKeyUp(key))
            {
                currentlyDown = false;
            }
        }
    }
}
