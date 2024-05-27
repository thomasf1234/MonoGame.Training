using MonoGame.Training.Constants;
using MonoGame.Training.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame.Training.Repositories
{
    public class EntityRepository : IEntityRepository
    {
        private readonly Queue<int> _entityIds;
        public EntityRepository()
        {
            _entityIds = new Queue<int>();
            for (int i = 0; i< EngineConstants.MaxEntities; ++i)
            {
                _entityIds.Enqueue(i);
            }
        }

        public Entity Create()
        {
            if (!_entityIds.Any())
            {
                throw new InvalidOperationException("All entities have been assigned");
            }

            var entityId = _entityIds.Dequeue();
            var entity = new Entity(entityId);

            return entity;
        }

        public void Destroy(Entity entity)
        {
            // TODO : Uniqueness check? Add a deleted flag on Entity?
            _entityIds.Enqueue(entity.Id);
        }
    }
}
