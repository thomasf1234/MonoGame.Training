﻿using Microsoft.Xna.Framework;
using MonoGame.Training.Components;
using MonoGame.Training.Entities;
using MonoGame.Training.Repositories;
using MonoGame.Training.Systems;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Training.DependencyInjection;

// https://badecho.com/index.php/2023/08/02/alpha-spritebatch/
namespace MonoGame.Training.Scenes
{
    public class TitleScene : Scene
    {
        private IResourceRepository _resourceRepository;
        private IInputRepository _inputRepository;
        private IEntityRepository _entityRepository;
        private IComponentRepository _componentRepository;
        private MenuSystem _menuSystem;
        private TextRenderSystem _textRenderSystem;
        private Game1 _game;

        public TitleScene(ServiceContainer serviceContainer) : base()
        {
            _game = serviceContainer.Get<Game1>();
            _resourceRepository = serviceContainer.Get<IResourceRepository>();
            _entityRepository = serviceContainer.Get<IEntityRepository>();
            _componentRepository = serviceContainer.Get<IComponentRepository>();
            _inputRepository = serviceContainer.Get<IInputRepository>();
        }

        protected override void OnLoading()
        {
            #region Initialise Systems
            _textRenderSystem = new TextRenderSystem(_componentRepository, _resourceRepository);
            _menuSystem = new MenuSystem(_componentRepository, _inputRepository);
            #endregion

            #region Main Menu Entities
            var demosMenuItemEntity = CreateMenuItemEntity("Demos", 0);
            var optionsMenuEntity = CreateMenuItemEntity("Options", 1);
            var exitMenuEntity = CreateMenuItemEntity("Exit", 2);

            var mainMenuEntities = new List<Entity>()
            {
                demosMenuItemEntity,
                optionsMenuEntity,
                exitMenuEntity
            };
            #endregion

            #region Demos Submenu Entities
            var demosTinyChaoGardenMenuItemEntity = CreateMenuItemEntity("Tiny Chao Garden", 0);
            var demosPongMenuItemEntity = CreateMenuItemEntity("Pong", 1);
            var demosCollisionTestMenuItemEntity = CreateMenuItemEntity("Collision Test", 2);
            var demosPalletTownTestMenuItemEntity = CreateMenuItemEntity("Pallet Town", 3);
            var demosBackMenuItemEntity = CreateMenuItemEntity("Back", 4);

            var demosSubmenuEntities = new List<Entity>()
            {
                demosTinyChaoGardenMenuItemEntity,
                demosPongMenuItemEntity,
                demosCollisionTestMenuItemEntity,
                demosPalletTownTestMenuItemEntity,
                demosBackMenuItemEntity
            };
            #endregion

            #region Options Submenu Entities
            var optionsScreenMenuItemEntity = CreateMenuItemEntity("Screen", 0);
            var optionsBackMenuItemEntity = CreateMenuItemEntity("Back", 1);

            var optionsSubmenuEntities = new List<Entity>()
            {
                optionsScreenMenuItemEntity,
                optionsBackMenuItemEntity
            };
            #endregion

            #region Main Menu Events
            _componentRepository.SetComponent(demosMenuItemEntity.Id, new EventComponent()
            {
                OnActivate = () =>
                {
                    foreach (var entity in mainMenuEntities)
                    {
                        _menuSystem.Deregister(entity.Id);
                        _textRenderSystem.Deregister(entity.Id);
                    }

                    foreach (var entity in demosSubmenuEntities)
                    {
                        _menuSystem.Register(entity.Id);
                        _textRenderSystem.Register(entity.Id);
                    }
                }
            });

            _componentRepository.SetComponent(optionsMenuEntity.Id, new EventComponent()
            {
                OnActivate = () =>
                {
                    foreach (var entity in mainMenuEntities)
                    {
                        _menuSystem.Deregister(entity.Id);
                        _textRenderSystem.Deregister(entity.Id);
                    }

                    foreach (var entity in optionsSubmenuEntities)
                    {
                        _menuSystem.Register(entity.Id);
                        _textRenderSystem.Register(entity.Id);
                    }
                }
            });

            _componentRepository.SetComponent(exitMenuEntity.Id, new EventComponent()
            {
                OnActivate = () => Exit(2)
            });
            #endregion

            #region Demos Submenu Events
            _componentRepository.SetComponent(demosTinyChaoGardenMenuItemEntity.Id, new EventComponent()
            {
                OnActivate = () => Exit(1)
            });

            _componentRepository.SetComponent(demosPongMenuItemEntity.Id, new EventComponent()
            {
                OnActivate = () => Exit(3)
            });

            _componentRepository.SetComponent(demosCollisionTestMenuItemEntity.Id, new EventComponent()
            {
                OnActivate = () => Exit(4)
            });

            _componentRepository.SetComponent(demosPalletTownTestMenuItemEntity.Id, new EventComponent()
            {
                OnActivate = () => { }
            });

            _componentRepository.SetComponent(demosBackMenuItemEntity.Id, new EventComponent()
            {
                OnActivate = () =>
                {
                    foreach (var entity in demosSubmenuEntities)
                    {
                        _menuSystem.Deregister(entity.Id);
                        _textRenderSystem.Deregister(entity.Id);
                    }

                    foreach (var entity in mainMenuEntities)
                    {
                        _menuSystem.Register(entity.Id);
                        _textRenderSystem.Register(entity.Id);
                    }
                }
            });
            #endregion

            #region Options Submenu Events
            _componentRepository.SetComponent(optionsScreenMenuItemEntity.Id, new EventComponent()
            {
                OnActivate = () => { }
            });

            _componentRepository.SetComponent(optionsBackMenuItemEntity.Id, new EventComponent()
            {
                OnActivate = () =>
                {
                    foreach (var entity in optionsSubmenuEntities)
                    {
                        _menuSystem.Deregister(entity.Id);
                        _textRenderSystem.Deregister(entity.Id);
                    }

                    foreach (var entity in mainMenuEntities)
                    {
                        _menuSystem.Register(entity.Id);
                        _textRenderSystem.Register(entity.Id);
                    }
                }
            });
            #endregion

            #region Register Main Menu
            foreach (var entity in mainMenuEntities)
            {
                _menuSystem.Register(entity.Id);
                _textRenderSystem.Register(entity.Id);
            }
            #endregion
        }

        public override void Update(GameTime gameTime)
        {
            _menuSystem.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _game.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, samplerState: SamplerState.PointWrap);
            
            _textRenderSystem.Draw(_game.SpriteBatch);

            _game.SpriteBatch.End();
        }

        private Entity CreateMenuItemEntity(string text, int index)
        {
            var menuItemEntity = _entityRepository.Create();

            var textComponent = new TextComponent()
            {
                FontId = "Arial",
                Color = Color.Gray,
                Opacity = 1f,
                Text = text
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
                Position = new Vector2(100, 100 + 35 * index)
            };

            _componentRepository.SetComponent(menuItemEntity.Id, textComponent);
            _componentRepository.SetComponent(menuItemEntity.Id, meshComponent);
            _componentRepository.SetComponent(menuItemEntity.Id, transformComponent);

            return menuItemEntity;
        }
    }
}
