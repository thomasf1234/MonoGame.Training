using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Training.Helpers
{
    public class InputHelper
    {
        static KeyboardState currentKeyState;
        static KeyboardState previousKeyState;
        private long? _lastUpdateAtTicks;

        public void Update(GameTime gameTime)
        {
            var currentTicks = gameTime.TotalGameTime.Ticks;

            if (_lastUpdateAtTicks == null)
            {
                currentKeyState = Keyboard.GetState();
                _lastUpdateAtTicks = currentTicks;
            }
            else  if (currentTicks > _lastUpdateAtTicks)
            {
                previousKeyState = currentKeyState;
                currentKeyState = Keyboard.GetState();
                _lastUpdateAtTicks = currentTicks;
            }
        }

        public bool AnyKeyPressed(Keys[] keys)
        {
            foreach (var key in keys)
            {
                if (IsKeyPressed(key))
                {
                    return true;
                }
            }

            return false;
        }

        public bool AnyKeyDown(Keys[] keys)
        {
            foreach (var key in keys)
            {
                if (IsKeyDown(key))
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsKeyDown(Keys key)
        {
            return currentKeyState.IsKeyDown(key);
        }

        public bool IsKeyUp(Keys key)
        {
            return currentKeyState.IsKeyUp(key);
        }

        public bool IsKeyPressed(Keys key)
        {
            if (previousKeyState == null)
            {
                return currentKeyState.IsKeyDown(key);
            }
            else
            {
                return currentKeyState.IsKeyDown(key) && previousKeyState.IsKeyUp(key);
            }
        }

        public bool IsKeyReleased(Keys key)
        {
            if (previousKeyState == null)
            {
                return false;
            }
            else
            {
                return currentKeyState.IsKeyUp(key) && previousKeyState.IsKeyDown(key);
            }
        }

        public Vector2 GetMousePosition()
        {
            // TODO : Cleanup
            return new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
        }
    }
}
