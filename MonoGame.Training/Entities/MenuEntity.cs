
using MonoGame.Training.Components;

// TODO : Move Signature out of entity
namespace MonoGame.Training.Entities
{
    public class MenuEntity : Entity
    {
        public TransformComponent TransformComponent { get; set;}
        public MenuComponent MenuComponent { get; set; }
    }
}