
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using MonoGame.Training.Repositories;
using System;
using MonoGame.Training.Components;
using MonoGame.Training.Events;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using MonoGame.Training.Models;

namespace MonoGame.Training.Systems
{
    public class CollisionSystem : System
    {
        public CollisionSystem(IComponentRepository componentRepository) : base(componentRepository)
        {

        }

        // TODO : Collision system should determine all collisions and order, need to figure out how to tackle domino effect for fast objects
        // TODO : Collision occurs over time interval, so must not recognise a new collision event if collision is still happening
        public void Update(GameTime gameTime)
        {
            var entityCount = EntityIds.Count;

            var collisionEventsByIndex = new Dictionary<string, List<CollisionEvent>>();

            for (int i=0; i< entityCount; ++i)
            {
                var iEntityId = EntityIds[i];
                var iMeshComponent = _componentRepository.GetComponent<MeshComponent>(iEntityId);
                var iTransformComponent = _componentRepository.GetComponent<TransformComponent>(iEntityId);
                var iMotionComponent = _componentRepository.GetComponent<MotionComponent>(iEntityId);
                var iEventComponent = _componentRepository.GetComponent<EventComponent>(iEntityId);
                var iRigidBodyComponent = _componentRepository.GetComponent<RigidBodyComponent>(iEntityId);
                var iImpulseComponent = _componentRepository.GetComponent<ImpulseComponent>(iEntityId);

                // Set default impulses to 0
                iImpulseComponent.Impulses = new List<Impulse>()
                {
                    new Impulse() { Force = Vector2.Zero, ElapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds }
                };

                for (int j=0; j< entityCount; ++j)
                {
                    if (i == j)
                    {
                        // Skip ourselves
                        continue;
                    }

                    var uniqueIndex = j > i ? $"{i}-{j}" : $"{j}-{i}";

                    if (collisionEventsByIndex.ContainsKey(uniqueIndex))
                    {
                        // skip if collision exists for i,j
                        continue;
                    }

                    var jEntityId = EntityIds[j];
                    var jMeshComponent = _componentRepository.GetComponent<MeshComponent>(jEntityId);
                    var jTransformComponent = _componentRepository.GetComponent<TransformComponent>(jEntityId);
                    var jMotionComponent = _componentRepository.GetComponent<MotionComponent>(jEntityId);
                    var jEventComponent = _componentRepository.GetComponent<EventComponent>(jEntityId);
                    var jRigidBodyComponent = _componentRepository.GetComponent<RigidBodyComponent>(jEntityId);
                    var jImpulseComponent = _componentRepository.GetComponent<ImpulseComponent>(jEntityId);
                    jImpulseComponent.Impulses = new List<Impulse>()
                    {
                        new Impulse() { Force = Vector2.Zero, ElapsedSeconds = gameTime.ElapsedGameTime.TotalSeconds }
                    };

                    var collisionEvents = FindCollision(iMeshComponent, iTransformComponent, iMotionComponent,
                        jMeshComponent, jTransformComponent, jMotionComponent, gameTime);

                    collisionEventsByIndex[uniqueIndex] = collisionEvents;

                    if (collisionEvents.Any())
                    {
                        // Call event handlers (0 is from i, 1 is from j)
                        iEventComponent.OnCollision(collisionEvents[0]);
                        jEventComponent.OnCollision(collisionEvents[1]);

                        var tc = gameTime.TotalGameTime.TotalSeconds - collisionEvents[0].Time;
                        var dt = gameTime.ElapsedGameTime.TotalSeconds;
                        // Assume force is applied at end of this frame
                        var impulseDuration = (dt - tc);

                        // TODO REMOVE
                        // Only apply impulse to massive non-static bodies
                        if (iRigidBodyComponent.Mass > 0 && iRigidBodyComponent.Mass < float.PositiveInfinity)
                        {
                            // dot product of velocity with direction  to determine velocity along direction axis
                            var uf = Math.Abs(Vector2.Dot(iMotionComponent.Velocity, collisionEvents[0].Direction));
                            var f = ((float)(iRigidBodyComponent.Mass / impulseDuration)) * 2 * uf;
                            iImpulseComponent.Impulses = new List<Impulse>()
                            {
                                new Impulse() { Force = Vector2.Zero, ElapsedSeconds = tc },
                                new Impulse() { Force = f * collisionEvents[0].Direction, ElapsedSeconds = impulseDuration }
                            };
                        }

                        if (jRigidBodyComponent.Mass > 0 && jRigidBodyComponent.Mass < float.PositiveInfinity)
                        {
                            jImpulseComponent.Impulses = new List<Impulse>()
                            {
                                new Impulse() { Force = Vector2.Zero, ElapsedSeconds = tc },
                                new Impulse() { Force = collisionEvents[1].Direction, ElapsedSeconds = impulseDuration }
                            };
                        }
                    }
                }
            }

            if (collisionEventsByIndex.Any())
            {
                foreach (var item in collisionEventsByIndex)
                {
                    var key = item.Key;
                    var collisionEvents = item.Value;

                    foreach (var collisionEvent in collisionEvents)
                    {
                        Debug.WriteLine($"{key}: Collision at ({collisionEvent.Position.X}, {collisionEvent.Position.Y}) @ {collisionEvent.Time}s");
                    }
                }
            }
        }

        // TODO : Cache from position, velocity and acceleration values
        public List<CollisionEvent> FindCollision(
            MeshComponent meshComponentA, TransformComponent transformComponentA, MotionComponent motionComponentA,
            MeshComponent meshComponentB, TransformComponent transformComponentB, MotionComponent motionComponentB,
            GameTime gameTime
            )
        {
            Vector2 direction;
            var collisionEvents = new List<CollisionEvent>();

            if (motionComponentA.Velocity.Length() == 0 && motionComponentA.Acceleration.Length() == 0
                && motionComponentB.Velocity.Length() == 0 && motionComponentB.Acceleration.Length() == 0)
            {
                // Static objects won't collide
                return collisionEvents;
            }

            // ONLY functions for static B and constant velocity

            if (motionComponentA.Acceleration.Length() != 0 || motionComponentB.Acceleration.Length() != 0)
            {
                throw new NotImplementedException("Current limitation cannot use accelaration");
            }

            if (motionComponentB.Velocity.Length() != 0)
            {
                throw new NotImplementedException("Current limitation assumes a static second body");
            }

            if (meshComponentB.Edges.Count != 1)
            {
                throw new NotImplementedException("Current limitation assumes a single edge for second body");
            }

            var dt = gameTime.ElapsedGameTime.TotalSeconds;

            // 1) Calculate initial and final position for A trajectory
            var uX_A = motionComponentA.Velocity.X;
            var uY_A = motionComponentA.Velocity.Y;

            var sX1_A = transformComponentA.Position.X;
            var sY1_A = transformComponentA.Position.Y;
            var sX2_A = (uX_A * dt) + sX1_A;
            var sY2_A = (uY_A * dt) + sY1_A;

            // 2) Calculate y(x) for A
            // TODO : Broken for vertical line as m = infinity
            if (sX1_A == sX2_A)
            {
                throw new NotImplementedException("Cannot support vertical edges");
            }
            var m_A = (sY2_A - sY1_A) / (sX2_A - sX1_A);
            var c_A = sY1_A - (m_A * sX1_A);
            // y_A = (m_A * x_A) + c_A;

            // 3) Calculate y(x) for B
            var s1_B = transformComponentB.Position + meshComponentB.Vertices[meshComponentB.Edges[0].Item1];
            var s2_B = transformComponentB.Position + meshComponentB.Vertices[meshComponentB.Edges[0].Item2];

            var sX1_B = s1_B.X;
            var sY1_B = s1_B.Y;
            var sX2_B = s2_B.X;
            var sY2_B = s2_B.Y;

            if (gameTime.TotalGameTime.TotalSeconds > 5)
            {
                ;
            }

            double x;
            double y;

            if (sX1_B == sX2_B)
            {
                //throw new NotImplementedException("Cannot support vertical edges");
                // don't need y(x) for B
                x = sX1_B;
                y = (m_A * x) + c_A;

                // TODO : Parallel lne check
                              
                direction = uX_A > 0 ? new Vector2(-1, 0) : new Vector2(1, 0);
            }
            else if (sY1_B == sY2_B)
            {
                var m_B = (sY2_B - sY1_B) / (sX2_B - sX1_B);
                var c_B = sY1_B - (m_B * sX1_B);

                // 4) Check for intersection
                if (m_A == m_B)
                {
                    // Parallel lines won't intersect
                    // TODO : Compare if same line
                    return collisionEvents;
                }

                x = (c_B - c_A) / (m_A - m_B);
                y = sY1_B;

                direction = uY_A > 0 ? new Vector2(0, -1) : new Vector2(0, 1);
            }
            else
            {
                var m_B = (sY2_B - sY1_B) / (sX2_B - sX1_B);
                var c_B = sY1_B - (m_B * sX1_B);

                // 4) Check for intersection
                if (m_A == m_B)
                {
                    // Parallel lines won't intersect
                    // TODO : Compare if same line
                    return collisionEvents;
                }

                x = (c_B - c_A) / (m_A - m_B);
                y = (m_A * x) + c_A;

                // TODO : Work out correct value here
                throw new NotImplementedException("Direction calculation needs to be finished");
            }

            // 5) Check if intersection is correct
            var x_Valid = false;
            var y_Valid = false;            

            if (sX2_B > sX1_B)
            {
                if (x >= sX1_B && x <= sX2_B)
                {
                    x_Valid = true;
                }
            }

            if (sX2_B < sX1_B)
            {
                if (x >= sX2_B && x <= sX1_B)
                {
                    x_Valid = true;
                }
            }

            if (sX1_B == sX2_B)
            {
                if (x == sX1_B)
                {
                    x_Valid = true;
                }
            }

            if (sY2_B > sY1_B)
            {
                if (y >= sY1_B && y <= sY2_B)
                {
                    y_Valid = true;
                }
            }

            if (sY2_B < sY1_B)
            {
                if (y >= sY2_B && y <= sY1_B)
                {
                    y_Valid = true;
                }
            }

            if (sY1_B == sY2_B)
            {
                if (y == sY1_B)
                {
                    y_Valid = true;
                }
            }

            if (!x_Valid || !y_Valid)
            {
                // Intersection not valid within time frame
                return collisionEvents;
            }

            var t = (x - sX1_A) / uX_A;

            if (x == sX1_A && y == sY1_A)
            {
                // Already at position so collision has already happened
                //Debug.WriteLine($"Collision already ocurred at ({x}, {y}) @ {gameTime.TotalGameTime.TotalSeconds}s + {t}s");

                return collisionEvents;
            }

            if (t > 0)
            {
                //Debug.WriteLine($"Colliding at ({x},{y}) in {t}s @{gameTime.TotalGameTime.TotalSeconds + t}s: Current is ({sX1_A}, {sY1_A}), ElapsedSinceLast = {gameTime.ElapsedGameTime.TotalSeconds} Total = {gameTime.TotalGameTime.TotalSeconds}s");
            }

            if (t < 0 || t >= dt)
            {
                // Intersection not valid within time frame
                return collisionEvents;
            }

            // Find the direction of impact


            var collisionEventA = new CollisionEvent()
            {
                Position = new Vector2((float)x, (float)y),
                Direction = direction,
                Time = (float) (gameTime.TotalGameTime.TotalSeconds + t)
            };

            var collisionEventB = new CollisionEvent()
            {
                Position = new Vector2((float)x, (float)y),
                Direction = -direction,
                Time = (float)(gameTime.TotalGameTime.TotalSeconds + t)
            };

            collisionEvents.Add(collisionEventA);
            collisionEvents.Add(collisionEventB);

            return collisionEvents;
        }
    }
}