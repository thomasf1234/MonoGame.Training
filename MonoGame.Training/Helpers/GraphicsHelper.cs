using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.Training.Helpers
{
    public class GraphicsHelper
    {
        private readonly GraphicsDeviceManager _graphics;

        public GraphicsHelper(GraphicsDeviceManager graphics)
        {
            _graphics = graphics;
        }

        public Rectangle GetWindowBounds()
        {
            return _graphics.GraphicsDevice.Viewport.Bounds;
        }

        public GraphicsDeviceManager GetGraphicsDeviceManager()
        {
            return _graphics;
        }
    }
}
