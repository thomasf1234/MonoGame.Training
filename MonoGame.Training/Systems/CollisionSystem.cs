
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using MonoGame.Training.Repositories;
using System;
using MonoGame.Training.Components;
using MonoGame.Training.Events;
using System.Linq;
using System.Diagnostics;
using System.Threading;

namespace MonoGame.Training.Systems
{
    public class CollisionSystem : System
    {
        public CollisionSystem(IComponentRepository componentRepository) : base(componentRepository)
        {

        }

        public void Update(GameTime gameTime)
        {
            var entityCount = EntityIds.Count;

            var collisionEventsByIndex = new Dictionary<string, CollisionEvent>();

            for (int i=0; i< entityCount; ++i)
            {
                var iEntityId = EntityIds[i];
                var iMeshComponent = _componentRepository.GetComponent<MeshComponent>(iEntityId);
                var iTransformComponent = _componentRepository.GetComponent<TransformComponent>(iEntityId);
                var iMotionComponent = _componentRepository.GetComponent<MotionComponent>(iEntityId);
                var iEventComponent = _componentRepository.GetComponent<EventComponent>(iEntityId);


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


                    var collision = FindCollision(iMeshComponent, iTransformComponent, iMotionComponent,
                        jMeshComponent, jTransformComponent, jMotionComponent, gameTime);
                        //(float)gameTime.ElapsedGameTime.TotalSeconds);

                    if (collision == null)
                    {
                        // Set null to record no collision - useful to reduce repeat check
                        collisionEventsByIndex[uniqueIndex] = null;
                    }
                    else
                    {
                        var iCollisionEvent = new CollisionEvent()
                        {
                            EntityIds = new Tuple<Guid, Guid>(iEntityId, jEntityId),
                            Position = collision.Item1,
                            Time = (float)gameTime.TotalGameTime.TotalSeconds + collision.Item2
                        };
                        collisionEventsByIndex[uniqueIndex] = iCollisionEvent;

                        var jCollisionEvent = new CollisionEvent()
                        {
                            EntityIds = new Tuple<Guid, Guid>(jEntityId, iEntityId),
                            Position = collision.Item1,
                            Time = (float)gameTime.TotalGameTime.TotalSeconds + collision.Item2
                        };

                        // Call event handlers
                        iEventComponent.OnCollision(iCollisionEvent);
                        jEventComponent.OnCollision(jCollisionEvent);
                    }
                }

                // For testing purposes
                //break;
            }

            if (collisionEventsByIndex.Any())
            {
                foreach (var item in collisionEventsByIndex)
                {
                    var key = item.Key;
                    var collisionEvent = item.Value;

                    if (collisionEvent != null)
                    {
                        //Debug.WriteLine($"{key}: Collision at ({collisionEvent.Position.X}, {collisionEvent.Position.Y}) @ {collisionEvent.Time}s");
                    }
                }
            }
        }

        // TODO : Cache from position, velocity and acceleration values
        public Tuple<Vector2, float>? FindCollision(
            MeshComponent meshComponentA, TransformComponent transformComponentA, MotionComponent motionComponentA,
            MeshComponent meshComponentB, TransformComponent transformComponentB, MotionComponent motionComponentB,
            GameTime gameTime
            )
        {
            if (motionComponentA.Velocity.Length() == 0 && motionComponentA.Acceleration.Length() == 0
                && motionComponentB.Velocity.Length() == 0 && motionComponentB.Acceleration.Length() == 0)
            {
                // Static objects won't collide
                return null;
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


            double x;
            double y;

            if (sX1_B == sX2_B)
            {
                //throw new NotImplementedException("Cannot support vertical edges");
                // don't need y(x) for B
                x = sX1_B;
                y = (m_A * x) + c_A;

                // TODO : Parallel lne check
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
                    return null;
                }

                x = (c_B - c_A) / (m_A - m_B);
                y = (m_A * x) + c_A;
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
                return null;
            }

            var t = (x - sX1_A) / uX_A;

            if (t >= 0)
            {
                //Debug.WriteLine($"Colliding at ({x},{y}) in {t}s @{gameTime.TotalGameTime.TotalSeconds + t}s: Current is ({sX1_A}, {sY1_A}), ElapsedSinceLast = {gameTime.ElapsedGameTime.TotalSeconds} Total = {gameTime.TotalGameTime.TotalSeconds}s");
            }

            if (t < 0 || t >= dt)
            {
                // Intersection not valid within time frame
                return null;
            }

            return new Tuple<Vector2, float>(new Vector2((float)x, (float)y), (float)t);
        }
    }
}