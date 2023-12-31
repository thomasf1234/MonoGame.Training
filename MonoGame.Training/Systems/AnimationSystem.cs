
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using MonoGame.Training.Repositories;
using System;
using MonoGame.Training.Components;

namespace MonoGame.Training.Systems
{
    public class AnimationSystem : System
    {
        public AnimationSystem(IComponentRepository componentRepository) : base(componentRepository)
        {
            
        }

        public void Update(GameTime gameTime)
        {
            foreach (var entityId in EntityIds)
            {
                var animationComponent = _componentRepository.GetComponent<AnimationComponent>(entityId);
                var graphicComponent = _componentRepository.GetComponent<ImageComponent>(entityId);

                var activeAnimation = animationComponent.ActiveAnimation;
                activeAnimation.ElapsedTime += gameTime.ElapsedGameTime;
                int frameIndex = (int)Math.Floor((activeAnimation.ElapsedTime.TotalSeconds / activeAnimation.FrameSpeed) % activeAnimation.FrameCount);
                var frameRectangle = new Rectangle((int)activeAnimation.Location.X + (activeAnimation.FrameWidth * frameIndex), (int)activeAnimation.Location.Y, activeAnimation.FrameWidth, activeAnimation.FrameHeight);
                graphicComponent.Rectangle = frameRectangle;
            }
        }
    }
}