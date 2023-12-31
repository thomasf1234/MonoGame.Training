using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Training.Repositories
{
    public interface IAssetRepository
    {
        public Texture2D GetTexture(string textureId);

        public void SetTexture(string textureId, Texture2D texture);

        public SpriteFont GetFont(string fontId);
        public void SetFont(string fontId, SpriteFont font);

        public Effect GetEffect(string effectId);
        public void SetEffect(string effectId, Effect effect);
    }
}
