using Microsoft.Xna.Framework;
using System;

namespace MonoGame.Training.Models
{
    public class Animation
    {
        public int FrameCount { get; set; }
        public Vector2 Location { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public float FrameSpeed { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public bool IsLooping { get; set; }
        public bool IsPlaying { get; private set; }

        public Animation(Vector2 location, int frameWidth, int frameHeight, int frameCount, float frameSpeed, bool isLooping)
        {
            Location = location;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            FrameCount = frameCount;
            FrameSpeed = frameSpeed;
            IsPlaying = false;
            IsLooping = isLooping;
            ElapsedTime = TimeSpan.Zero;
        }

        public void Play()
        {
            IsPlaying = true;
            ElapsedTime = TimeSpan.Zero;
        }

        public void Stop()
        {
            IsPlaying = false;
        }
    }
}
