using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Training.Components
{
    public class TextureComponent : Component
    {
        public Texture2D Texture { get; set; }
        public string TextureId { get; set; }
        public Rectangle Rectangle { get; set;}
        public float Opacity { get; set; } = 1.0f;

        public TextureComponent(Texture2D texture)
        {
            Texture = texture;
            Rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
        }
    }
}