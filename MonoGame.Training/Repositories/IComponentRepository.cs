using MonoGame.Training.Components;
using System;

namespace MonoGame.Training.Repositories
{
    public interface IComponentRepository
    {
        public T GetComponent<T>(Guid guid) where T : Component;

        public void SetComponent<T>(Guid guid, T component) where T : Component;
    }
}
