using Microsoft.Xna.Framework;
using MonoGame.Training.Components;
using MonoGame.Training.Constants;
using MonoGame.Training.Entities;
using MonoGame.Training.Models;
using MonoGame.Training.Repositories;
using MonoGame.Training.StateMachines;
using MonoGame.Training.Systems;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using MonoGame.Training.DependencyInjection;

namespace MonoGame.Training.Scenes
{
    //https://gamefromscratch.com/monogame-tutorial-audio/
    public class ChaoGardenScene : Scene
    {
        private IConfigurationRepository _configurationRepository;
        private IResourceRepository _resourceRepository;
        private IEntityRepository _entityRepository;
        private IComponentRepository _componentRepository;
        private IInputRepository _inputRepository;
        private MetricSystem _metricSystem;
        private MotionSystem _motionSystem;
        private AnimationSystem _animationSystem;
        private SpriteRenderSystem _renderSystem;
        private TextRenderSystem _textRenderSystem;
        private IChaoStateMachine _chaoStateMachine;
        private SoundSystem _soundSystem;
        private SoundComponent _tinyChaoGardenSoundComponent;
        private Matrix _scaleMatrix;

        private bool _paused;

        private Game1 _game;

        public ChaoGardenScene(ServiceContainer serviceContainer) : base()
        {
            _game = serviceContainer.Get<Game1>();
            _configurationRepository = serviceContainer.Get<IConfigurationRepository>(); ;
            _resourceRepository = serviceContainer.Get<IResourceRepository>();
            _entityRepository = serviceContainer.Get<IEntityRepository>();
            _componentRepository = serviceContainer.Get<IComponentRepository>();
            _inputRepository = serviceContainer.Get<IInputRepository>();
            _paused = false;
        }

        protected override void OnLoading()
        {
            var virtualWidth = 176;
            var virtualHeight = 160;
            AspectRatio = virtualWidth / (float) virtualHeight;

            var actualWidth = _game.GraphicsDevice.Viewport.Width; // PreferredBackBufferWidth;
            var actualHeight = _game.GraphicsDevice.Viewport.Height; //.PreferredBackBufferHeight;
            _scaleMatrix = Matrix.CreateScale(
                (float)actualHeight / virtualHeight, //(float)actualWidth / virtualWidth,
                (float)actualHeight / virtualHeight,
                1f);





            #region Load configuration
            var configuration = _configurationRepository.Load();
            #endregion

            #region Load Assets
            var backgroundMusicEntity = _entityRepository.Create();
            _tinyChaoGardenSoundComponent = new SoundComponent()
            {
                SoundId = "TinyChaoGarden_Theme",
                IsLooping = true,
                IsPaused = false,
                Background = true
            };
            _componentRepository.SetComponent<SoundComponent>(backgroundMusicEntity.Id, _tinyChaoGardenSoundComponent);
            _soundSystem = new SoundSystem(configuration, _componentRepository, _resourceRepository);
            _soundSystem.Register(backgroundMusicEntity.Id);


            var chaoGardenTexture = _resourceRepository.GetTexture("ChaoGarden");
            var chaoSpritesTexture = _resourceRepository.GetTexture("ChaoSprites");
            #endregion

            #region Initialise Systems
            _metricSystem = new MetricSystem(_componentRepository);
            _motionSystem = new MotionSystem(_componentRepository);
            _animationSystem = new AnimationSystem(_componentRepository);
            _renderSystem = new SpriteRenderSystem(_componentRepository, _game.SpriteBatch);
            _textRenderSystem = new TextRenderSystem(_componentRepository, _resourceRepository);
            #endregion

            #region Background Entity
            var backgroundEntity = _entityRepository.Create();
            var backgroundEntityTransformComponent = new TransformComponent()
            {
                Position = new Vector2(0, 0)
            };
            var backgroundEntityGraphicComponent = new TextureComponent(chaoGardenTexture);

            _componentRepository.SetComponent(backgroundEntity.Id, backgroundEntityTransformComponent);
            _componentRepository.SetComponent(backgroundEntity.Id, backgroundEntityGraphicComponent);

            _renderSystem.Register(backgroundEntity.Id);
            #endregion

            #region Chao Entity
            var chaoAnimations = new Dictionary<string, Animation>()
                    {
                        { "IdleDown", new Animation(new Vector2(0, 0), 21, 24, 1, 0.2f, true) },
                        { "IdleRight", new Animation(new Vector2(0, 24*1), 21, 24, 1, 0.2f, true) },
                        { "IdleLeft", new Animation(new Vector2(0, 24*2), 21, 24, 1, 0.2f, true) },
                        { "IdleUp", new Animation(new Vector2(0, 24*3), 21, 24, 1, 0.2f, true) },
                        { "WalkDown", new Animation(new Vector2(0, 24*4), 21, 24, 4, 0.2f, true) },
                        { "WalkRight", new Animation(new Vector2(0, 24*5), 21, 24, 4, 0.2f, true) },
                        { "WalkLeft", new Animation(new Vector2(0, 24*6), 21, 24, 4, 0.2f, true) },
                        { "WalkUp", new Animation(new Vector2(0, 24*7), 21, 24, 4, 0.2f, true) }
                    };

            var chaoEntity = new ChaoEntity(_entityRepository.Create().Id)
            {
                TransformComponent = new TransformComponent()
                {
                    Position = new Vector2(50, 100),
                    Rotation = (int)Direction.Down
                },
                MotionComponent = new MotionComponent()
                {
                    Velocity = Vector2.Zero,
                    Acceleration = Vector2.Zero
                },
                GraphicComponent = new TextureComponent(chaoSpritesTexture)
                {
                    Rectangle = new Rectangle(0, 0, 21, 24)
                },
                AnimationComponent = new AnimationComponent()
                {
                    ActiveAnimation = chaoAnimations["IdleDown"], //Move to initial state in state machine
                    Animations = chaoAnimations
                }
            };

            // Assign state machine
            var chaoStateMachine = new ChaoStateMachine();
            chaoStateMachine.Assign(chaoEntity);
            _chaoStateMachine = chaoStateMachine;

            _componentRepository.SetComponent(chaoEntity.Id, chaoEntity.TransformComponent);
            _componentRepository.SetComponent(chaoEntity.Id, chaoEntity.MotionComponent);
            _componentRepository.SetComponent(chaoEntity.Id, chaoEntity.GraphicComponent);
            _componentRepository.SetComponent(chaoEntity.Id, chaoEntity.AnimationComponent);

            _motionSystem.Register(chaoEntity.Id);
            _animationSystem.Register(chaoEntity.Id);
            _renderSystem.Register(chaoEntity.Id);
            #endregion

            #region Metrics Entity
            var metricsEntity = _entityRepository.Create();

            var metricsTextComponent = new TextComponent()
            {
                FontId = "Arial",
                Color = Color.White,
                Opacity = 1f,
                Text = ""
            };

            var metricsMeshComponent = new MeshComponent()
            {
                Vertices = new List<Vector2>()
                    {
                        new Vector2(0, 0),
                        new Vector2(100, 0),
                        new Vector2(100, 35),
                        new Vector2(0, 35)
                    }
            };

            var metricsTransformComponent = new TransformComponent()
            {
                Position = new Vector2(10, 10)
            };

            _componentRepository.SetComponent(metricsEntity.Id, metricsTextComponent);
            _componentRepository.SetComponent(metricsEntity.Id, metricsMeshComponent);
            _componentRepository.SetComponent(metricsEntity.Id, metricsTransformComponent);

            _metricSystem.Register(metricsEntity.Id);
            _textRenderSystem.Register(metricsEntity.Id);
            #endregion
        }

        protected override void OnActive()
        {

        }

        public override void Update(GameTime gameTime)
        {
            _metricSystem.Update(gameTime);

            // Pause
            if (_inputRepository.IsKeyPressed(Keys.P))
            {
                _paused = !_paused;
               // _tinyChaoGardenSoundComponent.IsPaused = _paused;
            }

            _soundSystem.Update(gameTime);

            if (_paused)
            {
                return;
            }

            // inputSystem triggers state transition to WALKING state, passing rotation/direction and velocity properties
            // state transition back to IDLE, passing rotation/direction

            // Bitwise operation on key states to determine action

            var keysPressed = 0b0000;

            if (_inputRepository.AnyKeyDown(new Keys[] { Keys.W, Keys.Up }))
                keysPressed = keysPressed | 0b0001;
            if (_inputRepository.AnyKeyDown(new Keys[] { Keys.S, Keys.Down }))
                keysPressed = keysPressed | 0b0010;
            if (_inputRepository.AnyKeyDown(new Keys[] { Keys.A, Keys.Left }))
                keysPressed = keysPressed | 0b0100;
            if (_inputRepository.AnyKeyDown(new Keys[] { Keys.D, Keys.Right }))
                keysPressed = keysPressed | 0b1000;

            switch (keysPressed)
            {
                case 0b0000:
                    _chaoStateMachine.TriggerStopAsync();
                    break;
                case 0b0001:
                    _chaoStateMachine.TriggerWalkAsync(Direction.Up);
                    break;
                case 0b0010:
                    _chaoStateMachine.TriggerWalkAsync(Direction.Down);
                    break;
                case 0b0100:
                    _chaoStateMachine.TriggerWalkAsync(Direction.Left);
                    break;
                case 0b1000:
                    _chaoStateMachine.TriggerWalkAsync(Direction.Right);
                    break;
            }

            // Update Systems
            _motionSystem.Update(gameTime);
            _animationSystem.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw call
            _game.GraphicsDevice.Clear(Color.Black);
            if (_paused)
            {
                var grayScaleEffect = _resourceRepository.GetEffect("GrayScale");
                //grayScaleEffect.CurrentTechnique.Passes[0].Apply();

                // TODO : Pass spriteBatch to each scene
                // Shader must be applied at sprite batch begin, affecting all sprites drawn during spritebatch.End(). SortMode.Deferred waits until spriteBatch.End() to draw
                _game.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, samplerState: SamplerState.PointWrap, effect: grayScaleEffect, transformMatrix: _scaleMatrix);

            }
            else
            {
                _game.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, samplerState: SamplerState.PointWrap, transformMatrix: _scaleMatrix);
            }

            _renderSystem.Draw(gameTime);

            var font = _resourceRepository.GetFont("Arial");

            if (_paused)
            {
                // https://devblogs.microsoft.com/xamarin/building-your-first-game-with-monogame-finishing-the-app/

                // Measure the text so we can center it correctly
                string pausedText = "PAUSED";
                // Calculate the center of the screen
                var center = new Vector2(200, 200); //_graphics.GraphicsDevice.Viewport.Bounds.Center.ToVector2();
                var v = new Vector2(font.MeasureString(pausedText).X / 2, 0);
                _game.SpriteBatch.DrawString(font, pausedText, center - v + new Vector2(2, 2), Color.Black);
                _game.SpriteBatch.DrawString(font, pausedText, center - v, Color.White);
            }

            _textRenderSystem.Draw(_game.SpriteBatch);

            _game.SpriteBatch.End();
        }
    }
}
