using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace MonoGame.Training.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private readonly Dictionary<string, Texture2D> _texturesById;
        private readonly Dictionary<string, SpriteFont> _fontsById;
        private readonly Dictionary<string, Effect> _effectsById;
        private readonly Dictionary<string, Song> _songsById;


        public AssetRepository()
        {
            _texturesById = new Dictionary<string, Texture2D>();
            _fontsById = new Dictionary<string, SpriteFont>();
            _effectsById = new Dictionary<string, Effect>();
            _songsById = new Dictionary<string, Song>();
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

        public Song GetSong(string songId)
        {
            return _songsById[songId];
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

        public void SetSong(string songId, Song song)
        {
            _songsById.Add(songId, song);
        }
    }
}
