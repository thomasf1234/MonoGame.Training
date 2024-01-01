using Microsoft.Xna.Framework;
using System;

namespace MonoGame.Training.Events
{
    public class CollisionEvent
    {
        public Vector2 Position { get; set; }
        public Vector2 Direction { get; set; }
        public float Time { get; set; }
    }
}
