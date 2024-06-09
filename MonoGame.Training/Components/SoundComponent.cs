using Microsoft.Xna.Framework.Media;

namespace MonoGame.Training.Components
{
    public class SoundComponent : Component
    {
        public string SoundId { get; set; }
        public bool IsLooping { get; set; }
        public bool IsPaused { get; set; }
        public bool Background { get; set; }
    }
}