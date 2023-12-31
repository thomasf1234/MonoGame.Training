using MonoGame.Training.Events;
using System;

namespace MonoGame.Training.Components
{
    public class EventComponent : Component
    {
        public Action<CollisionEvent> OnCollision { get; set; }
    }
}