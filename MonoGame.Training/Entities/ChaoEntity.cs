
using MonoGame.Training.Components;

namespace MonoGame.Training.Entities
{
    public class ChaoEntity : Entity
    {
        public ChaoEntity(int id) : base(id)
        {

        }

        public TransformComponent TransformComponent { get; set;}
        public MotionComponent MotionComponent { get; set; }
        public TextureComponent GraphicComponent { get; set;}
        public AnimationComponent AnimationComponent { get; set;}
    }
}