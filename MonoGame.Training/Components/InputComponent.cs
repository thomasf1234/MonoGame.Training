using System;

namespace MonoGame.Training.Components
{
    public class InputComponent : Component
    {
        public Action<float, float> OnMouseMove { get; set; }
    }
}