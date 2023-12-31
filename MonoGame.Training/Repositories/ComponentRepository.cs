using System;
using System.Collections.Generic;
using MonoGame.Training.Components;

namespace MonoGame.Training.Repositories
{
    public class ComponentRepository : IComponentRepository
    {
        private Dictionary<Type, Dictionary<Guid, Component>> _components;
        public ComponentRepository()
        {
            _components = new Dictionary<Type, Dictionary<Guid, Component>>();
        }

        public T GetComponent<T>(Guid guid) where T : Component
        {
            var type = typeof(T);

            return (T)_components[type][guid];
        }

        public void SetComponent<T>(Guid guid, T component) where T : Component
        {
            var type = typeof(T);

            if (!_components.TryGetValue(type, out var c))
            {
                c = new Dictionary<Guid, Component>();
                _components[type] = c;
            }

            c[guid] = component;
        }
    }
}