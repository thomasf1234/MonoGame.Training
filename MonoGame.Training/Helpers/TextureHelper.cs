using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Training.Helpers
{
    public class TextureHelper
    {
        private readonly GraphicsDevice _graphicsDevice;

        public TextureHelper(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public Texture2D GetColouredTexture(Color color)
        {
            var texture = new Texture2D(_graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            texture.SetData(new[] { color });

            return texture;
        }
    }
}
