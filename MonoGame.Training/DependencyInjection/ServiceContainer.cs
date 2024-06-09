using System;
using System.Collections.Generic;

namespace MonoGame.Training.DependencyInjection
{
    public class ServiceContainer
    {
        private readonly Dictionary<Type, object> _container;

        public ServiceContainer()
        {
            _container = new Dictionary<Type, object>();
        }

        public void Set<T>(T service)
        {
            var type = typeof(T);

            if (_container.ContainsKey(type))
            {
                throw new InvalidOperationException($"Service of type {type} already assigned");
            }

            _container[type] = service;
        }

        public T Get<T>()
        {
            var type = typeof(T);

            return (T)_container[type];
        }
    }
}
