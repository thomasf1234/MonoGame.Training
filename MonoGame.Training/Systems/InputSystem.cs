
using Microsoft.Xna.Framework;
using MonoGame.Training.Repositories;
using MonoGame.Training.Components;
using MonoGame.Training.Helpers;

namespace MonoGame.Training.Systems
{
    public class InputSystem : System
    {
        private readonly InputHelper _inputHelper;
        private readonly GraphicsHelper _graphicsHelper;

        public InputSystem(IComponentRepository componentRepository, InputHelper inputHelper, GraphicsHelper graphicsHelper) : base(componentRepository)
        {
            _inputHelper = inputHelper;
            _graphicsHelper = graphicsHelper;
        }

        public void Update(GameTime gameTime)
        {
            var mousePosition = _inputHelper.GetMousePosition();
            var windowBounds = _graphicsHelper.GetWindowBounds();

            var gameX = mousePosition.X - windowBounds.Left;
            var gameY = mousePosition.Y - windowBounds.Top;

            if (gameX < 0)
                gameX = 0;

            if (gameX > windowBounds.Width)
                gameX = windowBounds.Width;

            if (gameY < 0)
                gameY = 0;

            if (gameY > windowBounds.Height)
                gameY = windowBounds.Height;

            foreach (var entityId in EntityIds)
            {
                var inputComponent = _componentRepository.GetComponent<InputComponent>(entityId);

                if (inputComponent.OnMouseMove != null)
                {
                    inputComponent.OnMouseMove.Invoke(gameX, gameY);
                }
            }
        }

        protected override void OnDeregister()
        {
            base.OnDeregister();
        }
    }
}