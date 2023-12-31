
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using MonoGame.Training.Repositories;
using Microsoft.Xna.Framework.Graphics;
using System;
using MonoGame.Training.Components;

namespace MonoGame.Training.Systems
{
    public class RenderSystem : System
    {
        public RenderSystem(IComponentRepository componentRepository) : base(componentRepository)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var entityId in EntityIds)
            {
                var transformComponent = _componentRepository.GetComponent<TransformComponent>(entityId);
                var imageComponent = _componentRepository.GetComponent<ImageComponent>(entityId);

                // TODO Explore layer depth
                // TODO Move scale into config
                //spriteBatch.Draw(imageComponent.Texture, transformComponent.Position, imageComponent.Rectangle, Color.White * imageComponent.Opacity, 0f, Vector2.Zero, new Vector2(5, 5), SpriteEffects.None, 0f);
                //spriteBatch.Draw(imageComponent.Texture, transformComponent.Position, imageComponent.Rectangle, Color.White * imageComponent.Opacity, 0f, Vector2.Zero, new Vector2(2, 2), SpriteEffects.None, 0f);
                spriteBatch.Draw(imageComponent.Texture, transformComponent.Position, imageComponent.Rectangle, Color.White * imageComponent.Opacity, 0f, Vector2.Zero, new Vector2(1, 1), SpriteEffects.None, 0f);

            }
        }
    }
}