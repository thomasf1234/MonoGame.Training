
using Microsoft.Xna.Framework;
using MonoGame.Training.Repositories;
using System;
using MonoGame.Training.Components;

namespace MonoGame.Training.Systems
{
    public class MetricSystem : System
    {
        public MetricSystem(IComponentRepository componentRepository) : base(componentRepository)
        {

        }

        public void Update(GameTime gameTime)
        {
            var frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var entityId in EntityIds)
            {
                var textComponent = _componentRepository.GetComponent<TextComponent>(entityId);

                textComponent.Text = $"{Math.Round(frameRate, 2)}FPS";
            }
        }
    }
}