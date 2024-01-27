using MonoGame.Training.Events;
using System;

namespace MonoGame.Training.Components
{
    // TODO : Maybe move all events onto single component?
    public class EventComponent : Component
    {
        public Action<CollisionEvent> OnCollision { get; set; }
        public Action OnActivate { get; set; }
    }
}