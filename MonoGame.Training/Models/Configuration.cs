using System;

namespace MonoGame.Training.Models
{
    public class Configuration
    {
        //public int ScreenWidth { get; set; }
        //public int ScreenHeight { get; set; }
        public bool Fullscreen { get; set; }
        public int RefreshRate { get; set; }
        public float Volume { get; set; }
        public static Configuration Instance => instance.Value;

        private static readonly Lazy<Configuration> instance = new Lazy<Configuration>(() => new Configuration());
        private Configuration()
        {
            Fullscreen = false;
            RefreshRate = 60;
            Volume = 1f;
        }
    }
}
