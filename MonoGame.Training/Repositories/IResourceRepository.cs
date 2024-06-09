using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace MonoGame.Training.Repositories
{
    public interface IResourceRepository
    {
        public Texture2D GetTexture(string textureId);

        public void SetTexture(string textureId, Texture2D texture);

        public SpriteFont GetFont(string fontId);
        public void SetFont(string fontId, SpriteFont font);

        public Effect GetEffect(string effectId);
        public void SetEffect(string effectId, Effect effect);

        public Song GetSong(string songId);
        public void SetSong(string songId, Song song);
    }
}
