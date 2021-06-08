using System;

using Microsoft.Xna.Framework.Input;

namespace ReeGame.Controllers
{
    class KeyMapping
    {
        Action action;
        Keys key;
        bool pressed;

        bool currentlyDown;

        public KeyMapping(Action action, Keys key, bool pressed)
        {
            this.action = action;
            this.key = key;
            this.pressed = pressed;

            currentlyDown = false;
        }

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
