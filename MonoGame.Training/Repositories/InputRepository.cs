using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Training.Repositories
{
    public class InputRepository : IInputRepository
    {
        private KeyboardState _currentKeyState;
        private KeyboardState _previousKeyState;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;
        private long? _lastUpdateAtTicks;

        public void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState)
        {
            var currentTicks = gameTime.TotalGameTime.Ticks;

            if (_lastUpdateAtTicks == null)
            {
                _currentKeyState = keyboardState;
                _currentMouseState = mouseState;
                _lastUpdateAtTicks = currentTicks;
            }
            else if (currentTicks > _lastUpdateAtTicks)
            {
                _previousKeyState = _currentKeyState;
                _currentKeyState = keyboardState;
                _previousMouseState = _currentMouseState;
                _currentMouseState = mouseState;
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
            return _currentKeyState.IsKeyDown(key);
        }

        public bool IsKeyUp(Keys key)
        {
            return _currentKeyState.IsKeyUp(key);
        }

        public bool IsKeyPressed(Keys key)
        {
            if (_previousKeyState == null)
            {
                return _currentKeyState.IsKeyDown(key);
            }
            else
            {
                return _currentKeyState.IsKeyDown(key) && _previousKeyState.IsKeyUp(key);
            }
        }

        public bool IsKeyReleased(Keys key)
        {
            if (_previousKeyState == null)
            {
                return false;
            }
            else
            {
                return _currentKeyState.IsKeyUp(key) && _previousKeyState.IsKeyDown(key);
            }
        }

        public Vector2 GetMousePosition()
        {
            // TODO : Cleanup
            return new Vector2(_currentMouseState.X, _currentMouseState.Y);
        }
    }
}
