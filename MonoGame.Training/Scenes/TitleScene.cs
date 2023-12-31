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
using System.Threading;

// https://badecho.com/index.php/2023/08/02/alpha-spritebatch/
namespace MonoGame.Training.Scenes
{
    public class TitleScene : Scene
    {
        private IAssetRepository _assetRepository;
        private InputHelper _inputHelper;
        private IComponentRepository _componentRepository;
        private MenuSystem _menuSystem;
        private TextRenderSystem _textRenderSystem;

        private List<Entity> _mainMenuEntities;
        private List<Entity> _optionsMenuEntities;


        public TitleScene(IAssetRepository assetRepository, IComponentRepository componentRepository, InputHelper inputHelper)
        {
            _assetRepository = assetRepository;
            _componentRepository = componentRepository;
            _inputHelper = inputHelper;

        }

        protected override void OnLoading()
        {
            _mainMenuEntities = new List<Entity>();
            var menuItems = new List<string>()
            {
                "Continue",
                "Pong",
                "Options",
                "Other",
                "Exit"
            };

            for (int i = 0; i < menuItems.Count; ++i)
            {
                var menuItem = menuItems[i];

                var menuItemEntity = new Entity() { Id = Guid.NewGuid() };
                _mainMenuEntities.Add(menuItemEntity);

                var textComponent = new TextComponent()
                {
                    FontId = "Arial",
                    Color = Color.Gray,
                    Opacity = 1f,
                    Text = menuItem
                };

                var meshComponent = new MeshComponent()
                {
                    Vertices = new List<Vector2>()
                    {
                        new Vector2(0, 0),
                        new Vector2(100, 0),
                        new Vector2(100, 35),
                        new Vector2(0, 35)
                    }
                };

                var transformComponent = new TransformComponent()
                {
                    Position = new Vector2(100, 100 + 35 * i)
                };

                _componentRepository.SetComponent(menuItemEntity.Id, textComponent);
                _componentRepository.SetComponent(menuItemEntity.Id, meshComponent);
                _componentRepository.SetComponent(menuItemEntity.Id, transformComponent);
            }

            _componentRepository.SetComponent(_mainMenuEntities[0].Id, new OnActivateComponent()
            {
                Action = () => Exit(1)
            });


            _componentRepository.SetComponent(_mainMenuEntities[1].Id, new OnActivateComponent()
            {
                Action = () => Exit(3)
            });

            _componentRepository.SetComponent(_mainMenuEntities[2].Id, new OnActivateComponent()
            {
                Action = () =>
                {
                    foreach (var entity in _mainMenuEntities)
                    {
                        _menuSystem.Deregister(entity.Id);
                        _textRenderSystem.Deregister(entity.Id);
                    }

                    foreach (var entity in _optionsMenuEntities)
                    {
                        _menuSystem.Register(entity.Id);
                        _textRenderSystem.Register(entity.Id);
                    }
                }
            });

            _componentRepository.SetComponent(_mainMenuEntities[3].Id, new OnActivateComponent()
            {
                Action = () => { }
            });

            _componentRepository.SetComponent(_mainMenuEntities[4].Id, new OnActivateComponent()
            {
                Action = () => Exit(2)
            });

            _textRenderSystem = new TextRenderSystem(_componentRepository, _assetRepository);
            _menuSystem = new MenuSystem(_componentRepository, _inputHelper);
            
            foreach (var entity in _mainMenuEntities)
            {
                _textRenderSystem.Register(entity.Id);
                _menuSystem.Register(entity.Id);
            }

            // Delay to see loading screen
            //Thread.Sleep(2000);




            // Options Submenu

            _optionsMenuEntities = new List<Entity>();

            var optionsMenuItems = new List<string>()
            {
                "Screen",
                "Back"
            };

            for (int i = 0; i < optionsMenuItems.Count; ++i)
            {
                var menuItem = optionsMenuItems[i];

                var menuItemEntity = new Entity() { Id = Guid.NewGuid() };
                _optionsMenuEntities.Add(menuItemEntity);

                var textComponent = new TextComponent()
                {
                    FontId = "Arial",
                    Color = Color.Gray,
                    Opacity = 1f,
                    Text = menuItem
                };

                var meshComponent = new MeshComponent()
                {
                    Vertices = new List<Vector2>()
                    {
                        new Vector2(0, 0),
                        new Vector2(100, 0),
                        new Vector2(100, 35),
                        new Vector2(0, 35)
                    }
                };

                var transformComponent = new TransformComponent()
                {
                    Position = new Vector2(100, 100 + 35 * i)
                };

                _componentRepository.SetComponent(menuItemEntity.Id, textComponent);
                _componentRepository.SetComponent(menuItemEntity.Id, meshComponent);
                _componentRepository.SetComponent(menuItemEntity.Id, transformComponent);
            }

            _componentRepository.SetComponent(_optionsMenuEntities[0].Id, new OnActivateComponent()
            {
                Action = () => { }
            });

            _componentRepository.SetComponent(_optionsMenuEntities[1].Id, new OnActivateComponent()
            {
                Action = () =>
                {
                    foreach (var entity in _optionsMenuEntities)
                    {
                        _menuSystem.Deregister(entity.Id);
                        _textRenderSystem.Deregister(entity.Id);
                    }

                    foreach (var entity in _mainMenuEntities)
                    {
                        _menuSystem.Register(entity.Id);
                        _textRenderSystem.Register(entity.Id);
                    }
                }
            });

        }


        public override void Update(GameTime gameTime)
        {
            _menuSystem.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, samplerState: SamplerState.PointWrap);
            
            _textRenderSystem.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
