using MonoGame.Training.Components;

namespace MonoGame.Training.Repositories
{
    public interface IComponentRepository
    {
        public T GetComponent<T>(int entityId) where T : Component;

        public void SetComponent<T>(int entityId, T component) where T : Component;
    }
}
