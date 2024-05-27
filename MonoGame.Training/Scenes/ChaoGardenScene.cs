using Microsoft.Xna.Framework;
using MonoGame.Training.Components;
using MonoGame.Training.Constants;
using MonoGame.Training.Entities;
using MonoGame.Training.Models;
using MonoGame.Training.Repositories;
using MonoGame.Training.StateMachines;
using MonoGame.Training.Systems;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Training.Helpers;
using System.Diagnostics;
using Microsoft.Xna.Framework.Media;

namespace MonoGame.Training.Scenes
{
    //https://gamefromscratch.com/monogame-tutorial-audio/
    public class ChaoGardenScene : Scene
    {
        private IAssetRepository _assetRepository;
        private IEntityRepository _entityRepository;
        private IComponentRepository _componentRepository;
        private InputHelper _inputHelper;
        private GraphicsHelper _graphicsHelper;
        private MetricSystem _metricSystem;
        private MotionSystem _motionSystem;
        private AnimationSystem _animationSystem;
        private SpriteRenderSystem _renderSystem;
        private TextRenderSystem _textRenderSystem;
        private IChaoStateMachine _chaoStateMachine;
        private MusicSystem _musicSystem;
        private SoundComponent _tinyChaoGardenSoundComponent;

        private bool _paused;

        public ChaoGardenScene(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, IAssetRepository assetRepository, IEntityRepository entityRepository, IComponentRepository componentRepository, InputHelper inputHelper, GraphicsHelper graphicsHelper) : base(spriteBatch, graphicsDevice)
        {
            _assetRepository = assetRepository;
            _entityRepository = entityRepository;
            _componentRepository = componentRepository;
            _inputHelper = inputHelper;
            _graphicsHelper = graphicsHelper;
            _paused = false;
        }

        protected override void OnLoading()
        {
            var backgroundMusicEntity = _entityRepository.Create();
            var tinyChaoGardenSong = _assetRepository.GetSong("Placeholder_TinyChaoGarden_Theme");
            _tinyChaoGardenSoundComponent = new SoundComponent()
            {
                Song = tinyChaoGardenSong,
                IsLooping = true,
                IsPaused = false
            };
            _componentRepository.SetComponent<SoundComponent>(backgroundMusicEntity.Id, _tinyChaoGardenSoundComponent);
            _musicSystem = new MusicSystem(_componentRepository);
            _musicSystem.Register(backgroundMusicEntity.Id);


            var chaoGardenTexture = _assetRepository.GetTexture("ChaoGarden");
            var chaoSpritesTexture = _assetRepository.GetTexture("ChaoSprites");

            // Initialise Entities
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

            var backgroundEntity = new BackgroundEntity(_entityRepository.Create().Id)
            {
                TransformComponent = new TransformComponent()
                {
                    Position = new Vector2(0, 0)
                },
                GraphicComponent = new TextureComponent(chaoGardenTexture)
            };

            // Persist data
            _componentRepository.SetComponent(chaoEntity.Id, chaoEntity.TransformComponent);
            _componentRepository.SetComponent(chaoEntity.Id, chaoEntity.MotionComponent);
            _componentRepository.SetComponent(chaoEntity.Id, chaoEntity.GraphicComponent);
            _componentRepository.SetComponent(chaoEntity.Id, chaoEntity.AnimationComponent);

            _componentRepository.SetComponent(backgroundEntity.Id, backgroundEntity.TransformComponent);
            _componentRepository.SetComponent(backgroundEntity.Id, backgroundEntity.GraphicComponent);

            _componentRepository.SetComponent(metricsEntity.Id, metricsTextComponent);
            _componentRepository.SetComponent(metricsEntity.Id, metricsMeshComponent);
            _componentRepository.SetComponent(metricsEntity.Id, metricsTransformComponent);

            // Intialise systems
            _metricSystem = new MetricSystem(_componentRepository);
            _metricSystem.Register(new List<int>() { metricsEntity.Id });

            _motionSystem = new MotionSystem(_componentRepository);
            _motionSystem.Register(new List<int>() { chaoEntity.Id });

            _animationSystem = new AnimationSystem(_componentRepository);
            _animationSystem.Register(new List<int>() { chaoEntity.Id });

            _renderSystem = new SpriteRenderSystem(_componentRepository, SpriteBatch);
            _renderSystem.Register(new List<int>() { backgroundEntity.Id, chaoEntity.Id });

            _textRenderSystem = new TextRenderSystem(_componentRepository, _assetRepository);
            _textRenderSystem.Register(new List<int>() { metricsEntity.Id });


        }

        protected override void OnActive()
        {

        }

        public override void Update(GameTime gameTime)
        {
            _metricSystem.Update(gameTime);

            // Pause
            if (_inputHelper.IsKeyPressed(Keys.P))
            {
                _paused = !_paused;
                _tinyChaoGardenSoundComponent.IsPaused = _paused;
            }

            _musicSystem.Update(gameTime);

            if (_paused)
            {
                return;
            }

            // inputSystem triggers state transition to WALKING state, passing rotation/direction and velocity properties
            // state transition back to IDLE, passing rotation/direction

            // Bitwise operation on key states to determine action

            var keysPressed = 0b0000;

            if (_inputHelper.AnyKeyDown(new Keys[] { Keys.W, Keys.Up }))
                keysPressed = keysPressed | 0b0001;
            if (_inputHelper.AnyKeyDown(new Keys[] { Keys.S, Keys.Down }))
                keysPressed = keysPressed | 0b0010;
            if (_inputHelper.AnyKeyDown(new Keys[] { Keys.A, Keys.Left }))
                keysPressed = keysPressed | 0b0100;
            if (_inputHelper.AnyKeyDown(new Keys[] { Keys.D, Keys.Right }))
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
            var graphicsDevice = _graphicsHelper.GetGraphicsDeviceManager().GraphicsDevice;

            // Draw call
            graphicsDevice.Clear(Color.Black);
            if (_paused)
            {
                var grayScaleEffect = _assetRepository.GetEffect("GrayScale");
                //grayScaleEffect.CurrentTechnique.Passes[0].Apply();

                // TODO : Pass spriteBatch to each scene
                // Shader must be applied at sprite batch begin, affecting all sprites drawn during spritebatch.End(). SortMode.Deferred waits until spriteBatch.End() to draw
                SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, samplerState: SamplerState.PointWrap, effect: grayScaleEffect);

            }
            else
            {
                SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, samplerState: SamplerState.PointWrap);
            }

            _renderSystem.Draw(gameTime);

            var font = _assetRepository.GetFont("Arial");

            if (_paused)
            {
                // https://devblogs.microsoft.com/xamarin/building-your-first-game-with-monogame-finishing-the-app/

                // Measure the text so we can center it correctly
                string pausedText = "PAUSED";
                // Calculate the center of the screen
                var center = new Vector2(200, 200); //_graphics.GraphicsDevice.Viewport.Bounds.Center.ToVector2();
                var v = new Vector2(font.MeasureString(pausedText).X / 2, 0);
                SpriteBatch.DrawString(font, pausedText, center - v + new Vector2(2, 2), Color.Black);
                SpriteBatch.DrawString(font, pausedText, center - v, Color.White);
            }

            _textRenderSystem.Draw(SpriteBatch);

            SpriteBatch.End();
        }
    }
}
