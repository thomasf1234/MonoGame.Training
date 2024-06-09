
using Microsoft.Xna.Framework;
using MonoGame.Training.Repositories;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Training.Components;
using System.Collections.Generic;

namespace MonoGame.Training.Systems
{
    // https://community.monogame.net/t/solved-drawing-primitives-and-spritebatch/10015
    // https://gamedev.stackexchange.com/questions/36026/does-every-entity-in-an-xna-game-need-its-own-basiceffect-instance
    public class RenderSystem : System
    {
        private readonly SpriteBatch _spriteBatch;

        public RenderSystem(IComponentRepository componentRepository, SpriteBatch spriteBatch) : base(componentRepository)
        {
            _spriteBatch = spriteBatch;
        }

        public void Draw(GameTime gameTime)
        {
            var textureCache = new Dictionary<string, Texture2D>();



            // TODO : group by textureId
            foreach (var entityId in EntityIds)
            {
                var transformComponent = _componentRepository.GetComponent<TransformComponent>(entityId);
                var textureComponent = _componentRepository.GetComponent<TextureComponent>(entityId);

                // TODO Explore layer depth
                // TODO Move scale into config
                //spriteBatch.Draw(imageComponent.Texture, transformComponent.Position, imageComponent.Rectangle, Color.White * imageComponent.Opacity, 0f, Vector2.Zero, new Vector2(5, 5), SpriteEffects.None, 0f);
                //spriteBatch.Draw(imageComponent.Texture, transformComponent.Position, imageComponent.Rectangle, Color.White * imageComponent.Opacity, 0f, Vector2.Zero, new Vector2(2, 2), SpriteEffects.None, 0f);
                _spriteBatch.Draw(textureComponent.Texture, transformComponent.Position, textureComponent.Rectangle, Color.White * textureComponent.Opacity, 0f, Vector2.Zero, new Vector2(1, 1), SpriteEffects.None, 0f);
            }
        }
    }
}