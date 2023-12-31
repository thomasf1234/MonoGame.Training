using Microsoft.Xna.Framework;

namespace MonoGame.Training.Components
{
    public class TextComponent : Component
    {
        public string FontId { get; set; }
        public Color Color { get; set; }
        public string Text { get; set; }
        public float Opacity { get; set; } = 1.0f;
    }
}