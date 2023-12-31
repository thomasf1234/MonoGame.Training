using Microsoft.Xna.Framework;

namespace MonoGame.Training.Components
{
    public class TransformComponent : Component
    {
        public Vector2 Position { get; set; }
        public int Rotation { get; set; } 
        public Vector2 Scale { get; set; }
    }
}