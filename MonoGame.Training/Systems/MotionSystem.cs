
using Microsoft.Xna.Framework;
using MonoGame.Training.Repositories;
using System;
using MonoGame.Training.Components;

namespace MonoGame.Training.Systems
{
    public class MotionSystem : System
    {
        public MotionSystem(IComponentRepository componentRepository) : base(componentRepository)
        {

        }

        public void Update(GameTime gameTime)
        {
            foreach (var entityId in EntityIds)
            {
                var transformComponent = _componentRepository.GetComponent<TransformComponent>(entityId);
                var motionComponent = _componentRepository.GetComponent<MotionComponent>(entityId);

                var uX = motionComponent.Velocity.X;
                var uY = motionComponent.Velocity.Y;
                var aX = motionComponent.Acceleration.X;
                var aY = motionComponent.Acceleration.Y;
                var dt = gameTime.ElapsedGameTime.TotalSeconds;

                var vX = uX + (aX * dt);
                var vY = uY + (aY * dt);

                var sX = (uX * dt) + 0.5 * (aX * dt * dt);
                var sY = (uY * dt) + 0.5 * (aY * dt * dt);

                var newVelocity = new Vector2((float)vX, (float)vY);
                var newPosition = Vector2.Add(transformComponent.Position, new Vector2((float)sX, (float)sY));

                motionComponent.Velocity = newVelocity;
                transformComponent.Position = newPosition;
            }
        }
    }
}