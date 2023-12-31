using Microsoft.Xna.Framework;
using MonoGame.Training.Repositories;
using System;
using System.Collections.Generic;

namespace MonoGame.Training.Systems
{
    public abstract class System
    {
        public List<Guid> EntityIds { get; }
        protected readonly IComponentRepository _componentRepository;

        public System(IComponentRepository componentRepository)
        {
            EntityIds = new List<Guid>();
            _componentRepository = componentRepository;
        }

        public void Register(Guid entityId)
        {
            EntityIds.Add(entityId);
            OnRegister();
        }

        public void Register(List<Guid> entityIds)
        {
            foreach (var entityId in entityIds)
            {
                Register(entityId);
            }
        }

        public void Deregister(Guid entityId)
        {
            EntityIds.Remove(entityId);
            OnDeregister();
        }

        public void Deregister(List<Guid> entityIds)
        {
            foreach (var entityId in entityIds)
            {
                Deregister(entityId);
            }
        }

        protected virtual void OnRegister()
        {

        }

        protected virtual void OnDeregister()
        {

        }
    }
}
