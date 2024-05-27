using MonoGame.Training.Entities;

namespace MonoGame.Training.Repositories
{
    public interface IEntityRepository
    {
        public Entity Create();

        public void Destroy(Entity entity);
    }
}
