using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoGame.Training.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private readonly Dictionary<string, Texture2D> _texturesById;
        private readonly Dictionary<string, SpriteFont> _fontsById;
        private readonly Dictionary<string, Effect> _effectsById;

        public AssetRepository()
        {
            _texturesById = new Dictionary<string, Texture2D>();
            _fontsById = new Dictionary<string, SpriteFont>();
            _effectsById = new Dictionary<string, Effect>();
        }

        public SpriteFont GetFont(string fontId)
        {
            return _fontsById[fontId];
        }

        public Texture2D GetTexture(string textureId)
        {
            return _texturesById[textureId];
        }

        public Effect GetEffect(string effectId)
        {
            return _effectsById[effectId];
        }

        public void SetFont(string fontId, SpriteFont font)
        {
            _fontsById.Add(fontId, font);
        }

        public void SetTexture(string textureId, Texture2D texture)
        {
            _texturesById.Add(textureId, texture);
        }

        public void SetEffect(string effectId, Effect effect)
        {
            _effectsById.Add(effectId, effect);
        }
    }
}
