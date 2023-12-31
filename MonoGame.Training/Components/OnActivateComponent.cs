using System;

namespace MonoGame.Training.Components
{
    public class OnActivateComponent : Component
    {
        public Action Action { get; set; }
    }
}