using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Training.Constants;
using Stateless;
using System.Diagnostics;

namespace MonoGame.Training.Scenes
{
    public abstract class Scene
    {
        public string Name { get; set; }
        // TODO : Move to own class
        public Vector2 CameraSize { get; set; }
        public int ExitCode { get; private set; }
        private StateMachine<SceneState, SceneTrigger> _stateMachine;
        private StateMachine<SceneState, SceneTrigger>.TriggerWithParameters<int> _exitTrigger;

        public Scene()
        {
            _stateMachine = new StateMachine<SceneState, SceneTrigger>(SceneState.Unloaded);

            _exitTrigger = _stateMachine.SetTriggerParameters<int>(SceneTrigger.Exit);

            _stateMachine.Configure(SceneState.Unloaded)
                .Permit(SceneTrigger.StartLoading, SceneState.Loading)
                .OnEntry(() => OnUnloaded());

            _stateMachine.Configure(SceneState.Loading)
                .Permit(SceneTrigger.FinishLoading, SceneState.Loaded)
                .OnEntry(() => {
                    OnLoading();
                    _stateMachine.FireAsync(SceneTrigger.FinishLoading);
                });

            _stateMachine.Configure(SceneState.Loaded)
                .Permit(SceneTrigger.StartUnloading, SceneState.Unloading)
                .Permit(SceneTrigger.Enter, SceneState.Active)
                .OnEntry(() => OnLoaded());

            _stateMachine.Configure(SceneState.Active)
                .Ignore(SceneTrigger.Enter)
                .Permit(SceneTrigger.Exit, SceneState.Exited)
                .OnEntry(() => OnActive());

            _stateMachine.Configure(SceneState.Exited)
                .Ignore(SceneTrigger.Exit)
                .Permit(SceneTrigger.StartUnloading, SceneState.Unloading)
                .OnEntryFrom(_exitTrigger, (exitCode) =>
                {
                    ExitCode = exitCode;
                    OnExited();
                });

            _stateMachine.Configure(SceneState.Unloading)
                .Permit(SceneTrigger.FinishUnloading, SceneState.Unloaded)
                .OnEntry(() =>
                {
                    OnUnloading();
                    _stateMachine.FireAsync(SceneTrigger.FinishUnloading);
                });

            // This event will be invoked at the very end of the trigger handling, after the last entry action have been executed.
            _stateMachine.OnTransitionCompleted(OnTransitionCompleted);
        }

        public SceneState GetState()
        {
            return _stateMachine.State;
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
        public void Load()
        {
            _stateMachine.FireAsync(SceneTrigger.StartLoading);
        }

        public void Enter()
        {
            _stateMachine.FireAsync(SceneTrigger.Enter);
        }

        public void Exit(int exitCode)
        {
            _stateMachine.FireAsync(_exitTrigger, exitCode);
        }

        public void Unload()
        {
            _stateMachine.FireAsync(SceneTrigger.StartUnloading);
        }

        protected virtual void OnUnloading()
        {
            Debug.WriteLine($"Scene {Name}: OnUnloading called");
        }

        protected virtual void OnUnloaded()
        {
            Debug.WriteLine($"Scene {Name}: OnUnloaded called");
        }

        protected virtual void OnLoading()
        {
            Debug.WriteLine($"Scene {Name}: OnLoading called");
        }

        protected virtual void OnLoaded()
        {
            Debug.WriteLine($"Scene {Name}: OnLoaded called");
        }

        protected virtual void OnActive()
        {
            Debug.WriteLine($"Scene {Name}: OnActive called");
        }

        protected virtual void OnExited()
        {
            Debug.WriteLine($"Scene {Name}: OnExited called");
        }

        private void OnTransitionCompleted(StateMachine<SceneState, SceneTrigger>.Transition transition)
        {
            Debug.WriteLine($"Scene {Name}: received trigger {transition.Trigger} and transitioned from {transition.Source} to {transition.Destination}");
        }

    }
}
