
using MonoGame.Training.Components;

namespace MonoGame.Training.Entities
{
    public class BackgroundEntity : Entity
    {
        public BackgroundEntity(int id) : base(id)
        {

        }

        public TransformComponent TransformComponent { get; set;}
        public TextureComponent GraphicComponent { get; set;}
    }
}