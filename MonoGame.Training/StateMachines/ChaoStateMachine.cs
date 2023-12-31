using Microsoft.Xna.Framework;
using MonoGame.Training.Constants;
using MonoGame.Training.Entities;
using MonoGame.Training.Exceptions;
using Stateless;
using Stateless.Graph;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MonoGame.Training.StateMachines
{
    public class ChaoStateMachine : IChaoStateMachine
    {
        //private readonly ILogger<ChaoStateMachine> _logger;
        private StateMachine<ChaoState, ChaoTrigger> _stateMachine;
        private StateMachine<ChaoState, ChaoTrigger>.TriggerWithParameters<Direction> _walkTrigger;

        private ChaoEntity _chaoEntity;
        private bool _hasAssigned;

        public ChaoStateMachine()
        {
            _hasAssigned = false;
        }

        public void Assign(ChaoEntity chaoEntity)
        {
            _hasAssigned = true;
            _chaoEntity = chaoEntity;
            _stateMachine = new StateMachine<ChaoState, ChaoTrigger>(ChaoState.Idle);

            _walkTrigger = _stateMachine.SetTriggerParameters<Direction>(ChaoTrigger.Walk);

            _stateMachine.Configure(ChaoState.Idle)
                .Ignore(ChaoTrigger.Stop)
                .Permit(ChaoTrigger.Walk, ChaoState.Walking)
                .OnEntryAsync(() => OnIdleAsync());

            _stateMachine.Configure(ChaoState.Walking)
                .Permit(ChaoTrigger.Stop, ChaoState.Idle)
                .IgnoreIf(_walkTrigger, (direction) => InDirection(direction))
                .PermitReentryIf(_walkTrigger, (direction) => !InDirection(direction))
                .OnEntryFromAsync(_walkTrigger, (direction) => OnWalkingAsync(direction));

            // This event will be invoked at the very end of the trigger handling, after the last entry action have been executed.
            _stateMachine.OnTransitionCompletedAsync(OnTransitionCompletedAsync);
        }

        public ChaoState GetState()
        {
            return _stateMachine.State;
        }

        public bool InDirection(Direction direction)
        {
            return direction == (Direction)_chaoEntity.TransformComponent.Rotation;
        }

        public async Task TriggerWalkAsync(Direction direction)
        {
            if (!_hasAssigned)
            {
                throw new NotAssignedException();
            }

            await _stateMachine.FireAsync(_walkTrigger, direction);
        }

        public async Task TriggerStopAsync()
        {
            if (!_hasAssigned)
            {
                throw new NotAssignedException();
            }

            await _stateMachine.FireAsync(ChaoTrigger.Stop);
        }

        public string ExportToDotGraph()
        {
            return UmlDotGraph.Format(_stateMachine.GetInfo());
        }

        private async Task OnWalkingAsync(Direction direction)
        {
            _chaoEntity.TransformComponent.Rotation = (int)direction;
            var speed = 75f;

            switch (direction)
            {
                case Direction.Up:
                    _chaoEntity.MotionComponent.Velocity = new Vector2(0, -speed);
                    _chaoEntity.AnimationComponent.ActiveAnimation.Stop();
                    _chaoEntity.AnimationComponent.ActiveAnimation = _chaoEntity.AnimationComponent.Animations["WalkUp"];
                    _chaoEntity.AnimationComponent.ActiveAnimation.Play();
                    break;
                case Direction.Down:
                    _chaoEntity.MotionComponent.Velocity = new Vector2(0, speed);
                    _chaoEntity.AnimationComponent.ActiveAnimation.Stop();
                    _chaoEntity.AnimationComponent.ActiveAnimation = _chaoEntity.AnimationComponent.Animations["WalkDown"];
                    _chaoEntity.AnimationComponent.ActiveAnimation.Play();
                    break;
                case Direction.Left:
                    _chaoEntity.MotionComponent.Velocity = new Vector2(-speed, 0);
                    _chaoEntity.AnimationComponent.ActiveAnimation.Stop();
                    _chaoEntity.AnimationComponent.ActiveAnimation = _chaoEntity.AnimationComponent.Animations["WalkLeft"];
                    _chaoEntity.AnimationComponent.ActiveAnimation.Play();
                    break;
                case Direction.Right:
                    _chaoEntity.MotionComponent.Velocity = new Vector2(speed, 0);
                    _chaoEntity.AnimationComponent.ActiveAnimation.Stop();
                    _chaoEntity.AnimationComponent.ActiveAnimation = _chaoEntity.AnimationComponent.Animations["WalkRight"];
                    _chaoEntity.AnimationComponent.ActiveAnimation.Play();
                    break;
            }
        }

        private async Task OnIdleAsync()
        {
            _chaoEntity.MotionComponent.Velocity = Vector2.Zero;

            switch ((Direction)_chaoEntity.TransformComponent.Rotation)
            {
                case Direction.Up:
                    _chaoEntity.AnimationComponent.ActiveAnimation.Stop();
                    _chaoEntity.AnimationComponent.ActiveAnimation = _chaoEntity.AnimationComponent.Animations["IdleUp"];
                    _chaoEntity.AnimationComponent.ActiveAnimation.Play();
                    break;
                case Direction.Down:
                    _chaoEntity.AnimationComponent.ActiveAnimation.Stop();
                    _chaoEntity.AnimationComponent.ActiveAnimation = _chaoEntity.AnimationComponent.Animations["IdleDown"];
                    _chaoEntity.AnimationComponent.ActiveAnimation.Play();
                    break;
                case Direction.Left:
                    _chaoEntity.AnimationComponent.ActiveAnimation.Stop();
                    _chaoEntity.AnimationComponent.ActiveAnimation = _chaoEntity.AnimationComponent.Animations["IdleLeft"];
                    _chaoEntity.AnimationComponent.ActiveAnimation.Play();
                    break;
                case Direction.Right:
                    _chaoEntity.AnimationComponent.ActiveAnimation.Stop();
                    _chaoEntity.AnimationComponent.ActiveAnimation = _chaoEntity.AnimationComponent.Animations["IdleRight"];
                    _chaoEntity.AnimationComponent.ActiveAnimation.Play();
                    break;
            }
        }

        private async Task OnTransitionCompletedAsync(StateMachine<ChaoState, ChaoTrigger>.Transition transition)
        {
            Debug.WriteLine($"Chao {_chaoEntity.Id} received trigger {transition.Trigger} and transitioned from {transition.Source} to {transition.Destination}");
        }
    }
}
