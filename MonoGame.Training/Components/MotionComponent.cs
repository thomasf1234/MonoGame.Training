using Microsoft.Xna.Framework;

namespace MonoGame.Training.Components
{
    public class MotionComponent : Component
    {
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; } 
    }
}