
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using MonoGame.Training.Repositories;
using Microsoft.Xna.Framework.Graphics;
using System;
using MonoGame.Training.Components;
using System.Linq;

namespace MonoGame.Training.Systems
{
    // https://community.monogame.net/t/solved-drawing-primitives-and-spritebatch/10015
    // https://gamedev.stackexchange.com/questions/36026/does-every-entity-in-an-xna-game-need-its-own-basiceffect-instance
    public class PrimitiveRenderSystem : System
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly VertexBuffer _vertexBuffer;
        private readonly IndexBuffer _indexBuffer;
        private readonly BasicEffect _basicEffect;
        private Dictionary<Guid, int> _entityVertexIndexMap; // Figure out how to manage position  for register/deregister
        public PrimitiveRenderSystem(IComponentRepository componentRepository, GraphicsDevice graphicsDevice) : base(componentRepository)
        {
            _graphicsDevice = graphicsDevice;

            // TODO : Determine max size of vertex buffer
            _indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), 1000, BufferUsage.WriteOnly);
            _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), 1000, BufferUsage.WriteOnly);

            // TODO : Setup view and projection matrix
            Matrix worldMatrix = Matrix.Identity;
            Matrix viewMatrix = Matrix.Identity;
            Matrix projectionMatrix = Matrix.CreateTranslation(-0.5f, -0.5f, 0) * Matrix.CreateOrthographicOffCenter(_graphicsDevice.Viewport.X, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height, _graphicsDevice.Viewport.Y, 0, 1);

            _basicEffect = new BasicEffect(graphicsDevice);
            _basicEffect.View = viewMatrix;
            _basicEffect.Projection = projectionMatrix;
            _basicEffect.VertexColorEnabled = true;
        }

        // TODO update vertex buffer once per frame
        // TODO : Single vertex buffer and index buffer for all meshes, updated OnRegister and OnDeregister
        protected override void OnRegister(Guid entityId)
        {
            var meshComponent = _componentRepository.GetComponent<MeshComponent>(entityId);

            var vertices = new VertexPositionColor[meshComponent.Vertices.Count];
            var color = Color.Red;
            for (int i = 0; i < meshComponent.Vertices.Count(); ++i)
            {
                var vertex = meshComponent.Vertices[i];
                vertices[i] = new VertexPositionColor(new Vector3(vertex.X, -vertex.Y, 0), color);
            }

            var indices = new short[meshComponent.Triangles.Count() * 3];
            for (int i = 0; i < meshComponent.Triangles.Count(); ++i)
            {
                var triangle = meshComponent.Triangles[i];
                var indicesIndex = i * 3;

                indices[indicesIndex] = (short)triangle.Vertex1;
                indices[indicesIndex + 1] = (short)triangle.Vertex2;
                indices[indicesIndex + 2] = (short)triangle.Vertex3;
            }

            _indexBuffer.SetData(indices);
            _vertexBuffer.SetData<VertexPositionColor>(vertices);
        }

        public void Draw(GameTime gameTime)
        {
            foreach (var entityId in EntityIds)
            {
                var meshComponent = _componentRepository.GetComponent<MeshComponent>(entityId);
                var transformComponent = _componentRepository.GetComponent<TransformComponent>(entityId);

                // Draw Primitives only if mesh has the data
                if (meshComponent.Triangles?.Count > 0)
                {
                    _graphicsDevice.SetVertexBuffer(_vertexBuffer);
                    _graphicsDevice.Indices = _indexBuffer;

                    RasterizerState rasterizerState = new RasterizerState();
                    rasterizerState.CullMode = CullMode.None;
                    _graphicsDevice.RasterizerState = rasterizerState;

                    // TODO : Update basicEffect World matrix with TransformationComponent before drawing each object
                    _basicEffect.World = Matrix.CreateTranslation(transformComponent.Position.X, transformComponent.Position.Y, 0);

                    foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, meshComponent.Triangles.Count());
                    }
                }
            }
        }
    }
}