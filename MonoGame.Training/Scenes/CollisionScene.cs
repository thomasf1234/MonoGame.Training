using Microsoft.Xna.Framework;
using MonoGame.Training.Repositories;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Training.Helpers;
using MonoGame.Training.Components;
using MonoGame.Training.Entities;
using System;
using MonoGame.Training.Systems;
using System.Collections.Generic;
using MonoGame.Training.Models;
using System.Windows.Forms;
using MonoGame.Training.Factories;
using System.Linq;
using MonoGame.Training.Models.Geometry;

// https://badecho.com/index.php/2023/08/02/alpha-spritebatch/
namespace MonoGame.Training.Scenes
{
    public class CollisionScene : Scene
    {
        private RenderTarget2D _nativeRenderTarget;
        private IAssetRepository _assetRepository;
        private InputHelper _inputHelper;
        private IComponentRepository _componentRepository;

        private RenderSystem _renderSystem;
        private InputSystem _inputSystem;
        private PhysicsSystem _physicsSystem;
        private CollisionSystem _collisionSystem;


        private GraphicsHelper _graphicsHelper;

        private Rectangle _actualScreenRectangle;

        private float _scale;
        private BasicEffect _basicEffect;
        private VertexBuffer _vertexBuffer;
        private VertexPositionColor[] _vertices;
        private Polygon _polygon;

        public CollisionScene(IAssetRepository assetRepository, IComponentRepository componentRepository, InputHelper inputHelper, GraphicsHelper graphicsHelper)
        {
            _assetRepository = assetRepository;
            _componentRepository = componentRepository;
            _inputHelper = inputHelper;
            _graphicsHelper = graphicsHelper;
            _scale = 2f;
        }

        protected override void OnLoading()
        {
            // Initialization
            _actualScreenRectangle = new Rectangle(0, 0, (int)(370 * _scale), (int)(290 * _scale));

            Matrix worldMatrix = Matrix.Identity;
            Matrix viewMatrix = Matrix.Identity;
            Matrix projectionMatrix = Matrix.CreateTranslation(-0.5f, -0.5f, 0) * Matrix.CreateOrthographicOffCenter(_actualScreenRectangle.X, _actualScreenRectangle.Width, _actualScreenRectangle.Height, _actualScreenRectangle.Y, 0, 1);

            _basicEffect = new BasicEffect(_graphicsHelper.GetGraphicsDeviceManager().GraphicsDevice);
            _basicEffect.World = worldMatrix;
            _basicEffect.View = viewMatrix;
            _basicEffect.Projection = projectionMatrix;

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

            _vertices = new VertexPositionColor[_polygon.Triangles.Count*3];

            for (int i=0; i< _polygon.Triangles.Count; ++i)
            {
                var triangle = _polygon.Triangles[i];

                var index = i * 3;
                var color = Color.Red;

                _vertices[index] = new VertexPositionColor(new Vector3(triangle.Vertex1.X, -triangle.Vertex1.Y, 0), color);
                _vertices[index+1] = new VertexPositionColor(new Vector3(triangle.Vertex2.X, -triangle.Vertex2.Y, 0), color);
                _vertices[index+2] = new VertexPositionColor(new Vector3(triangle.Vertex3.X, -triangle.Vertex3.Y, 0), color);
            }

            _vertexBuffer = new VertexBuffer(_graphicsHelper.GetGraphicsDeviceManager().GraphicsDevice, typeof(VertexPositionColor), _vertices.Count(), BufferUsage.WriteOnly);
            _vertexBuffer.SetData<VertexPositionColor>(_vertices);
            
            _basicEffect.VertexColorEnabled = true;
        }

        public override void Update(GameTime gameTime)
        {
            var mouseXY = _inputHelper.GetMousePosition();

            _basicEffect.World = Matrix.CreateTranslation(mouseXY.X, mouseXY.Y, 0);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var graphicsDevice = _graphicsHelper.GetGraphicsDeviceManager().GraphicsDevice;

            // Draw call
            //graphicsDevice.SetRenderTarget(_nativeRenderTarget);
            graphicsDevice.Clear(Color.Black);

            graphicsDevice.SetVertexBuffer(_vertexBuffer);

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, _polygon.Triangles.Count());
            }

            // now render your game like you normally would, but if you change the render target somewhere,
            // make sure you set it back to this one and not the backbuffer
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, samplerState: SamplerState.PointWrap);
            /* _renderSystem.Draw(spriteBatch);
             _textRenderSystem.Draw(spriteBatch);*/
          /*  DrawLine(spriteBatch, new Vector2(mouseXY.X, mouseXY.Y), new Vector2(mouseXY.X + 50, mouseXY.Y), Color.Green);
            DrawLine(spriteBatch, new Vector2(mouseXY.X + 50, mouseXY.Y), new Vector2(mouseXY.X + 50, mouseXY.Y + 30), Color.Green);
            DrawLine(spriteBatch, new Vector2(mouseXY.X + 50, mouseXY.Y+30), new Vector2(mouseXY.X, mouseXY.Y + 30), Color.Green);
            DrawLine(spriteBatch, new Vector2(mouseXY.X, mouseXY.Y + 30), new Vector2(mouseXY.X, mouseXY.Y), Color.Green);*/



            spriteBatch.End();

            // after drawing the game at native resolution we can render _nativeRenderTarget to the backbuffer!
            // First set the GraphicsDevice target back to the backbuffer
           // graphicsDevice.SetRenderTarget(null);
            // RenderTarget2D inherits from Texture2D so we can render it just like a texture
          /*  spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(_nativeRenderTarget, _actualScreenRectangle, Color.White);
            spriteBatch.End(); */
        }

        private Texture2D _texture;
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
        }
    }
}
