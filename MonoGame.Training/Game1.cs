using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Training.Systems;
using MonoGame.Training.Entities;
using MonoGame.Training.Components;
using MonoGame.Training.Repositories;
using System.Collections.Generic;
using System;
using MonoGame.Training.StateMachines;
using MonoGame.Training.Constants;
using MonoGame.Training.Models;
using MonoGame.Training.Scenes;
using MonoGame.Training.Helpers;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

// https://www.youtube.com/watch?v=9Wcy4wffuJs (Static background + Camera)
// https://www.youtube.com/watch?v=Pu7VAIs2mpg (Snow Particles)
// https://www.youtube.com/watch?v=OLsiWxgONeM (Sprite Animation)
// https://gamedev.stackexchange.com/questions/173780/fastest-way-to-look-up-an-entity-with-a-set-of-components
// https://learn-monogame.github.io/how-to/fullscreen/ (Fullscreen / Border)
// https://mysteriousspace.com/2019/01/05/pixel-shaders-in-monogame-a-tutorial-of-sorts-for-2019/
// https://www.david-colson.com/2020/02/09/making-a-simple-ecs.html
namespace MonoGame.Training
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private IAssetRepository _assetRepository;
        private IComponentRepository _componentRepository;
        private InputHelper _inputHelper;
        private GraphicsHelper _graphicsHelper;

        private Dictionary<string, Scene> _scenesByName;
        private Scene _activeScene;
        private Stopwatch _loadingStopwatch; 

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferHeight = 160 * 5;
            _graphics.PreferredBackBufferWidth = 176 * 5;

            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            _loadingStopwatch = new Stopwatch();
        }

        protected override void Initialize()
        {
            // https://community.monogame.net/t/the-use-of-a-fixed-time-step/9143
            /*_graphics.SynchronizeWithVerticalRetrace = false; //Vsync
            this.IsFixedTimeStep = true; // true;
            int targetFPS = 60;
            this.TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / targetFPS);*/

            _graphics.SynchronizeWithVerticalRetrace = true; //Vsync
            this.IsFixedTimeStep = true; // default;
            int targetFPS = 60;
            this.TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / targetFPS);

            // TODO: Add your initialization logic here
            _assetRepository = new AssetRepository();
            _componentRepository = new ComponentRepository();
            _inputHelper = new InputHelper();
            _graphicsHelper = new GraphicsHelper(_graphics);
            _scenesByName = new Dictionary<string, Scene>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            var chaoGardenTexture = this.Content.Load<Texture2D>("ChaoGarden");
            var chaoSpritesTexture = this.Content.Load<Texture2D>("ChaoSprites");

            // Add assets to AssetRepository for use in Scenes
            _assetRepository.SetTexture("ChaoGarden", chaoGardenTexture);
            _assetRepository.SetTexture("ChaoSprites", chaoSpritesTexture);

            // Load Pong sprites
            var pongSpriteNames = new List<string>()
            {
                "Pong/Ball",
                "Pong/Net",
                "Pong/Paddle",
                "Pong/Score_0",
                "Pong/Score_1",
                "Pong/Score_2",
                "Pong/Score_3",
                "Pong/Score_4",
                "Pong/Score_5",
                "Pong/Score_6",
                "Pong/Score_7",
                "Pong/Score_8",
                "Pong/Score_9"
            };
            foreach (var spriteName in pongSpriteNames)
            {
                var texture = this.Content.Load<Texture2D>(spriteName);
                _assetRepository.SetTexture(spriteName, texture);
            }


            var arialFont = this.Content.Load<SpriteFont>("Arial");
            _assetRepository.SetFont("Arial", arialFont);

            var grayScaleEffect = this.Content.Load<Effect>("GrayScale");
            _assetRepository.SetEffect("GrayScale", grayScaleEffect);

            var alphaEffect = this.Content.Load<Effect>("Alpha");
            _assetRepository.SetEffect("Alpha", alphaEffect);
            // TODO : Add layers to game and draw priority
        }

        protected override void Update(GameTime gameTime)
        {
            _inputHelper.Update(gameTime);

            var titleSceneName = "Title";
            if (!_scenesByName.TryGetValue(titleSceneName, out var titleScene))
            {
                titleScene = new TitleScene(_assetRepository, _componentRepository, _inputHelper)
                {
                    Name = titleSceneName
                };
                _scenesByName[titleSceneName] = titleScene;
                _activeScene = titleScene;
                _loadingStopwatch.Restart();
                var loadTask = new Task(() => titleScene.Load());
                loadTask.Start();
            }

            var chaoGardenSceneName = "ChaoGarden";
            if (!_scenesByName.TryGetValue(chaoGardenSceneName, out var chaoGardenScene))
            {
                chaoGardenScene = new ChaoGardenScene(_assetRepository, _componentRepository, _inputHelper)
                {
                    Name = chaoGardenSceneName
                };
                _scenesByName[chaoGardenSceneName] = chaoGardenScene;
            }

            var pongSceneName = "Pong";
            if (!_scenesByName.TryGetValue(pongSceneName, out var pongScene))
            {
                pongScene = new PongScene(_assetRepository, _componentRepository, _inputHelper, _graphicsHelper)
                {
                    Name = pongSceneName
                };
                _scenesByName[pongSceneName] = pongScene;
            }

            switch (_activeScene.GetState())
            {
                case SceneState.Loaded:            
                    _activeScene.Enter();
                    _loadingStopwatch.Reset();
                    break;
                case SceneState.Active:
                    _activeScene.Update(gameTime);
                    break;
                case SceneState.Exited:
                    _loadingStopwatch.Restart();
                    var exitedScene = _activeScene;
                    var unloadTask = new Task(() => exitedScene.Unload());
                    unloadTask.Start();

                    // Process ExitCode
                    switch (_activeScene.ExitCode)
                    {
                        case 1:
                            var nextScene = _scenesByName["ChaoGarden"];
                            _activeScene = nextScene;
                            var loadTask = new Task(() => nextScene.Load());
                            loadTask.Start();
                            break;
                        case 2:
                            // TODO : Wait until all scenes have unloaded
                            Exit();
                            break;
                        case 3:
                            _activeScene = _scenesByName["Pong"];
                            _scenesByName["Pong"].CameraSize = new Vector2(370, 290);
                            var loadPongTask = new Task(() => _scenesByName["Pong"].Load());
                            loadPongTask.Start();

                            _graphics.IsFullScreen = false;
                            // TODO : pull scene graphics requests from scene, or allow scenes to change from inside
                            /*_graphics.PreferredBackBufferWidth = 370 * 2;
                            _graphics.PreferredBackBufferHeight = 290 * 2;*/
                            _graphics.PreferredBackBufferWidth = 370 * 1;
                            _graphics.PreferredBackBufferHeight = 290 * 1;
                            _graphics.ApplyChanges();
                            break;
                    }
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            if (_activeScene == null || _activeScene.GetState() != SceneState.Active)
            {
                // Only show loading screen if above threshold time
                // TODO : Move threshold time to config or constants
                if (_loadingStopwatch.Elapsed.TotalSeconds > 1)
                {
                    var font = _assetRepository.GetFont("Arial");

                    var loadingText = "Loading...";
                    var bottomCenterPosition = new Vector2(_graphics.GraphicsDevice.Viewport.Bounds.Right / 2, _graphics.GraphicsDevice.Viewport.Bounds.Bottom);
                    var loadingTextWidth = font.MeasureString(loadingText).X;
                    var loadingTextHeight = font.MeasureString(loadingText).Y;
                    var offsetVector = new Vector2(0, -100);

                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, samplerState: SamplerState.PointWrap);
                    _spriteBatch.DrawString(font, loadingText, bottomCenterPosition + new Vector2(-loadingTextWidth / 2, -loadingTextHeight) + offsetVector, Color.White);
                    _spriteBatch.End();
                }
            }
            else
            {
                _activeScene.Draw(_spriteBatch, gameTime);
            }

            base.Draw(gameTime);
        }
    }
}
