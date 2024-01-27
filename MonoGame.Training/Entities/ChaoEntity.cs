
using MonoGame.Training.Components;

namespace MonoGame.Training.Entities
{
    public class ChaoEntity : Entity
    {
        public TransformComponent TransformComponent { get; set;}
        public MotionComponent MotionComponent { get; set; }
        public TextureComponent GraphicComponent { get; set;}
        public AnimationComponent AnimationComponent { get; set;}
    }
}