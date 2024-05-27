using System;
using System.Collections.Generic;
using MonoGame.Training.Collections;
using MonoGame.Training.Components;
using MonoGame.Training.Constants;

namespace MonoGame.Training.Repositories
{
    public class ComponentRepository : IComponentRepository
    {
        private Dictionary<Type, PackedArray<Component>> _components;
        public ComponentRepository()
        {
            _components = new Dictionary<Type, PackedArray<Component>>();
        }

        public T GetComponent<T>(int entityId) where T : Component
        {
            var type = typeof(T);

            return (T)_components[type].Get(entityId);
        }

        public void SetComponent<T>(int entityId, T component) where T : Component
        {
            var type = typeof(T);

            if (!_components.TryGetValue(type, out var c))
            {
                c = new PackedArray<Component>(EngineConstants.MaxEntities);
                _components[type] = c;
            }

            c.Insert(entityId, component);
        }
    }
}