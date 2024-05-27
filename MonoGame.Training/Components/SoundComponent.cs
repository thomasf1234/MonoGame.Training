using Microsoft.Xna.Framework.Media;

namespace MonoGame.Training.Components
{
    public class SoundComponent : Component
    {
        public Song Song { get; set; }
        public bool IsLooping { get; set; }
        public bool IsPaused { get; set; }
    }
}