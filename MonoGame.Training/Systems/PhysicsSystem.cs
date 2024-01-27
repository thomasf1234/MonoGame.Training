
using Microsoft.Xna.Framework;
using MonoGame.Training.Repositories;
using System;
using MonoGame.Training.Components;
using System.Collections.Generic;
using MonoGame.Training.Models;
using System.Diagnostics;

namespace MonoGame.Training.Systems
{
    public class PhysicsSystem : System
    {
        public PhysicsSystem(IComponentRepository componentRepository) : base(componentRepository)
        {

        }

        public void Update(GameTime gameTime)
        {
            foreach (var entityId in EntityIds)
            {
                var rigidBodyComponent = _componentRepository.GetComponent<RigidBodyComponent>(entityId);
                var transformComponent = _componentRepository.GetComponent<TransformComponent>(entityId);
                var motionComponent = _componentRepository.GetComponent<MotionComponent>(entityId);
                var impulseComponent = _componentRepository.GetComponent<ImpulseComponent>(entityId);

                var dt = gameTime.ElapsedGameTime.TotalSeconds;

                var m = rigidBodyComponent.Mass;
                var u = motionComponent.Velocity;

                var v = u;

                Vector2 s = transformComponent.Position;

                foreach (var impulse in impulseComponent.Impulses)
                {
                    var f = impulse.Force;
                    var idt = impulse.ElapsedSeconds;

                    u = v;

                    // If no impulse then not in a collision, so update distance
                    if (f == Vector2.Zero)
                    {
                        s.X += (float)(u.X * idt);
                        s.Y += (float)(u.Y * idt);

                        //Debug.WriteLine($"s = ({s.X}, {s.Y}), v = ({v.X}, {v.Y}) @ {gameTime.TotalGameTime.TotalSeconds}s");
                    }
                    else
                    {
                        // Assume instantaneous acceleration during impulse so no change in displacement
                        
                        // Calculate final velocity
                        v = u + ((float)(idt / m) * f);
                        // Round to 0 when close
                        v.X = Math.Abs(v.X) < 0.001 ? 0 : v.X;
                        v.Y = Math.Abs(v.Y) < 0.001 ? 0 : v.Y;
                        //Debug.WriteLine($"Under Force: f = ({f.X}, {f.Y}) s = ({s.X}, {s.Y}), v = {v.Length()} ({v.X}, {v.Y}) @ {gameTime.TotalGameTime.TotalSeconds}s");
                    }
                }


                var newVelocity = v;
                var newPosition = s;

                motionComponent.Velocity = newVelocity;
                transformComponent.Position = newPosition;
            }
        }
    }
}