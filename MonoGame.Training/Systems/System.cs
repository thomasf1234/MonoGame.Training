using Microsoft.Xna.Framework;
using MonoGame.Training.Repositories;
using System;
using System.Collections.Generic;

namespace MonoGame.Training.Systems
{
    public abstract class System
    {
        public List<int> EntityIds { get; }
        protected readonly IComponentRepository _componentRepository;

        public System(IComponentRepository componentRepository)
        {
            EntityIds = new List<int>();
            _componentRepository = componentRepository;
        }

        public void Register(int entityId)
        {
            EntityIds.Add(entityId);
            OnRegister(entityId);
        }

        // TODO Only allow entity with fully setup components to register
        // TODO Set entity signature using EnumFlags
        public void Register(List<int> entityIds)
        {
            foreach (var entityId in entityIds)
            {
                Register(entityId);
            }
        }

        public void Deregister(int entityId)
        {
            EntityIds.Remove(entityId);
            OnDeregister(entityId);
        }

        public void Deregister(List<int> entityIds)
        {
            foreach (var entityId in entityIds)
            {
                Deregister(entityId);
            }
        }

        protected virtual void OnRegister(int entityId)
        {

        }

        protected virtual void OnDeregister(int entityId)
        {

        }
    }
}
