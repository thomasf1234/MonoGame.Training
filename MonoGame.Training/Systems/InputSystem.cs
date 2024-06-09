
using Microsoft.Xna.Framework;
using MonoGame.Training.Repositories;
using MonoGame.Training.Components;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Training.Systems
{
    public class InputSystem : System
    {
        private readonly IInputRepository _inputRepository;
        private readonly Viewport _viewport;

        public InputSystem(IComponentRepository componentRepository, IInputRepository inputRepository, Viewport viewport) : base(componentRepository)
        {
            _inputRepository = inputRepository;
            _viewport = viewport;
        }

        public void Update(GameTime gameTime)
        {
            var mousePosition = _inputRepository.GetMousePosition();
            var windowBounds = _viewport.Bounds;

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
    }
}