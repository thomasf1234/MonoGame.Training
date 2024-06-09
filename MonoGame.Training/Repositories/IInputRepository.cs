using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Training.Repositories
{
    public interface IInputRepository
    {
        public void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState);
        public bool AnyKeyPressed(Keys[] keys);
        public bool AnyKeyDown(Keys[] keys);
        public bool IsKeyDown(Keys key);
        public bool IsKeyUp(Keys key);
        public bool IsKeyPressed(Keys key);
        public bool IsKeyReleased(Keys key);
        public Vector2 GetMousePosition();
    }
}
