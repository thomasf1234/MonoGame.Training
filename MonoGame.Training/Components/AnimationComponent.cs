using MonoGame.Training.Models;
using System.Collections.Generic;

namespace MonoGame.Training.Components
{
    public class AnimationComponent : Component
    {
        public Dictionary<string, Animation> Animations { get; set; }
        public Animation ActiveAnimation { get; set; }
    }
}