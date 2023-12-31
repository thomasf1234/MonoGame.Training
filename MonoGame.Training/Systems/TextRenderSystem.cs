
using Microsoft.Xna.Framework;
using MonoGame.Training.Repositories;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Training.Components;
using System;

namespace MonoGame.Training.Systems
{
    public class TextRenderSystem : System
    {
        private readonly IAssetRepository _assetRepository;

        public TextRenderSystem(IComponentRepository componentRepository, IAssetRepository assetRepository) : base(componentRepository)
        {
            _assetRepository = assetRepository;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            // TODO : Text Fade IN/OUT
            /*if (_animStart == 0)
            {
                _animStart = gameTime.TotalGameTime.TotalSeconds;
            }

            var opacity = (float)(gameTime.TotalGameTime.TotalSeconds - _animStart);
            opacity = opacity > 1 ? 1f : opacity;*/

            foreach (var entityId in EntityIds)
            {
                var textComponent = _componentRepository.GetComponent<TextComponent>(entityId);
                var transformComponent = _componentRepository.GetComponent<TransformComponent>(entityId);
                var meshComponent = _componentRepository.GetComponent<MeshComponent>(entityId);

                var font = _assetRepository.GetFont(textComponent.FontId);
                var scale = 1f; //5f; // Figure out how to addess scale for drawString;
                //spriteBatch.DrawString(font, menuItem, new Vector2(x + 2, y + offsetY + 2), Color.Black * opacity); TODO: Add Shadow float and fade in/out
                spriteBatch.DrawString(font, textComponent.Text, transformComponent.Position, textComponent.Color * textComponent.Opacity, transformComponent.Rotation, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
        }
    }
}