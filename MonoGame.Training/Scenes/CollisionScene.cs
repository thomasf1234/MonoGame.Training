﻿using Microsoft.Xna.Framework;
using MonoGame.Training.Repositories;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Training.Helpers;
using MonoGame.Training.Components;
using MonoGame.Training.Systems;
using System.Collections.Generic;
using MonoGame.Training.Factories;
using MonoGame.Training.Models.Geometry;
using MonoGame.Training.DependencyInjection;

// https://badecho.com/index.php/2023/08/02/alpha-spritebatch/
// http://rbwhitaker.wikidot.com/index-and-vertex-buffers
namespace MonoGame.Training.Scenes
{
    public class CollisionScene : Scene
    {
        private RenderTarget2D _nativeRenderTarget;
        private IResourceRepository _resourceRepository;
        private IInputRepository _inputRepository;
        private IComponentRepository _componentRepository;
        private IEntityRepository _entityRepository;


        private PrimitiveRenderSystem _renderSystem;
        private InputSystem _inputSystem;
        private PhysicsSystem _physicsSystem;
        private CollisionSystem _collisionSystem;

        private Rectangle _actualScreenRectangle;

        private float _scale;
        private Polygon _polygon;

        private Game1 _game;

        public CollisionScene(ServiceContainer serviceContainer) : base()
        {
            _game = serviceContainer.Get<Game1>();
            _resourceRepository = serviceContainer.Get<IResourceRepository>();
            _entityRepository = serviceContainer.Get<IEntityRepository>();
            _componentRepository = serviceContainer.Get<IComponentRepository>();
            _inputRepository = serviceContainer.Get<IInputRepository>();
            _scale = 2f;
        }

        protected override void OnLoading()
        {
            // Initialization
            _actualScreenRectangle = new Rectangle(0, 0, (int)(370 * _scale), (int)(290 * _scale));

            #region Create polygon entity
            var geometryHelper = new GeometryHelper();
            var polygonFactory = new PolygonFactory(geometryHelper);

            var vertices = new List<Vector2>
            {
                new Vector2(0, 0),
                new Vector2(0, 50),
                new Vector2(50, 100),
                new Vector2(100, 50),
                new Vector2(100, 0)
            };

            _polygon = polygonFactory.Create(vertices);

            var polygon1Entity = _entityRepository.Create();
            var meshComponent = new MeshComponent()
            {
                Vertices = _polygon.Vertices,
                Edges = _polygon.Edges,
                Triangles = _polygon.Triangles
            };
            var transformComponent = new TransformComponent()
            {
                Position = Vector2.Zero
            };
            var inputComponent = new InputComponent()
            {
                OnMouseMove = (x, y) =>
                {
                    /*var maxY = (_graphicsHelper.GetWindowBounds().Height / _scale) - (p1PaddleTextureComponent.Rectangle.Height);

                    x = x / _scale;
                    y = y / _scale;
                    y = y > maxY ? maxY : y;*/

                    transformComponent.Position = new Vector2(x, y);
                }
            };

            _componentRepository.SetComponent(polygon1Entity.Id, meshComponent);
            _componentRepository.SetComponent(polygon1Entity.Id, transformComponent);
            _componentRepository.SetComponent(polygon1Entity.Id, inputComponent);
            #endregion

            #region Initialise Systems
            _renderSystem = new PrimitiveRenderSystem(_componentRepository, _game.GraphicsDevice);
            _inputSystem = new InputSystem(_componentRepository, _inputRepository, _game.GraphicsDevice.Viewport);
            #endregion

            #region Register Entities
            _inputSystem.Register(polygon1Entity.Id);
            _renderSystem.Register(polygon1Entity.Id);
            #endregion
        }

        public override void Update(GameTime gameTime)
        {
            _inputSystem.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw call
            //graphicsDevice.SetRenderTarget(_nativeRenderTarget);
            _game.GraphicsDevice.Clear(Color.Black);

            _renderSystem.Draw(gameTime);

            // now render your game like you normally would, but if you change the render target somewhere,
            // make sure you set it back to this one and not the backbuffer
            _game.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, samplerState: SamplerState.PointWrap);
            /* _renderSystem.Draw(spriteBatch);
             _textRenderSystem.Draw(spriteBatch);*/
            /*  DrawLine(spriteBatch, new Vector2(mouseXY.X, mouseXY.Y), new Vector2(mouseXY.X + 50, mouseXY.Y), Color.Green);
              DrawLine(spriteBatch, new Vector2(mouseXY.X + 50, mouseXY.Y), new Vector2(mouseXY.X + 50, mouseXY.Y + 30), Color.Green);
              DrawLine(spriteBatch, new Vector2(mouseXY.X + 50, mouseXY.Y+30), new Vector2(mouseXY.X, mouseXY.Y + 30), Color.Green);
              DrawLine(spriteBatch, new Vector2(mouseXY.X, mouseXY.Y + 30), new Vector2(mouseXY.X, mouseXY.Y), Color.Green);*/



            _game.SpriteBatch.End();

            // after drawing the game at native resolution we can render _nativeRenderTarget to the backbuffer!
            // First set the GraphicsDevice target back to the backbuffer
           // graphicsDevice.SetRenderTarget(null);
            // RenderTarget2D inherits from Texture2D so we can render it just like a texture
          /*  spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(_nativeRenderTarget, _actualScreenRectangle, Color.White);
            spriteBatch.End(); */
        }

/*        private Texture2D _texture;
        private Texture2D GetTexture(SpriteBatch spriteBatch)
        {
            if (_texture == null)
            {
                _texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                _texture.SetData(new[] { Color.White });
            }

            return _texture;
        }

        public void DrawLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(spriteBatch, point1, distance, angle, color, thickness);
        }

        public void DrawLine(SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(GetTexture(spriteBatch), point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }*/
    }
}

/*for (var i = 0; i < _VertexBuffers.Length; i++)
{
    GraphicsDevice.SetVertexBuffer(_VertexBuffers[i]);
    GraphicsDevice.Indices = _IndexBuffers[i];

    GraphicsDevice.DrawIndexedPrimitives(
      PrimitiveType.TriangleList,
      0,
      0,
      _Slices[i].PrimitiveCount);
}*/