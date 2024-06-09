
using Microsoft.Xna.Framework;
using MonoGame.Training.Repositories;
using MonoGame.Training.Components;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoGame.Training.Systems
{
    public class MenuSystem : System
    {
        private int _currentSelectionIndex;
        private readonly IInputRepository _inputRepository;
        public MenuSystem(IComponentRepository componentRepository, IInputRepository inputRepository) : base(componentRepository)
        {
            _currentSelectionIndex = 0;
            _inputRepository = inputRepository;
        }

        public void Update(GameTime gameTime)
        {
            if (_inputRepository.AnyKeyPressed(new Keys[] { Keys.S, Keys.Down }))
            {
                var newIndex = _currentSelectionIndex += 1;
                if (newIndex == EntityIds.Count)
                {
                    newIndex = 0;
                }

                _currentSelectionIndex = newIndex;
            }

            if (_inputRepository.AnyKeyPressed(new Keys[] { Keys.W, Keys.Up }))
            {
                var newIndex = _currentSelectionIndex -= 1;
                if (newIndex == -1)
                {
                    newIndex = EntityIds.Count - 1;
                }

                _currentSelectionIndex = newIndex;
            }

            if (_inputRepository.AnyKeyPressed(new Keys[] { Keys.Space, Keys.Enter }))
            {
                var entityId = EntityIds[_currentSelectionIndex];

                var eventComponent = _componentRepository.GetComponent<EventComponent>(entityId);
                eventComponent.OnActivate.Invoke();
            }

            for (int i = 0; i < EntityIds.Count; ++i)
            {
                var entityId = EntityIds[i];

                var textComponent = _componentRepository.GetComponent<TextComponent>(entityId);

                textComponent.Color = i == _currentSelectionIndex ? Color.White : Color.Gray;
            }
        }

        protected override void OnDeregister(int entityId)
        {
            base.OnDeregister(entityId);

            _currentSelectionIndex = 0;
        }
    }
}