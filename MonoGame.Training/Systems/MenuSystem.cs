
using Microsoft.Xna.Framework;
using MonoGame.Training.Repositories;
using MonoGame.Training.Components;
using MonoGame.Training.Helpers;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoGame.Training.Systems
{
    public class MenuSystem : System
    {
        private int _currentSelectionIndex;
        private readonly InputHelper _inputHelper;
        public MenuSystem(IComponentRepository componentRepository, InputHelper inputHelper) : base(componentRepository)
        {
            _currentSelectionIndex = 0;
            _inputHelper = inputHelper;
        }

        public void Update(GameTime gameTime)
        {
            if (_inputHelper.AnyKeyPressed(new Keys[] { Keys.S, Keys.Down }))
            {
                var newIndex = _currentSelectionIndex += 1;
                if (newIndex == EntityIds.Count)
                {
                    newIndex = 0;
                }

                _currentSelectionIndex = newIndex;
            }

            if (_inputHelper.AnyKeyPressed(new Keys[] { Keys.W, Keys.Up }))
            {
                var newIndex = _currentSelectionIndex -= 1;
                if (newIndex == -1)
                {
                    newIndex = EntityIds.Count - 1;
                }

                _currentSelectionIndex = newIndex;
            }

            if (_inputHelper.AnyKeyPressed(new Keys[] { Keys.Space, Keys.Enter }))
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

        protected override void OnDeregister(Guid entityId)
        {
            base.OnDeregister(entityId);

            _currentSelectionIndex = 0;
        }
    }
}