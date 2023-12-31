using Microsoft.Xna.Framework;
using MonoGame.Training.Repositories;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Training.Helpers;
using MonoGame.Training.Components;
using MonoGame.Training.Entities;
using System;
using MonoGame.Training.Systems;
using System.Collections.Generic;
using SharpDX.MediaFoundation;

// https://badecho.com/index.php/2023/08/02/alpha-spritebatch/
namespace MonoGame.Training.Scenes
{
    public class PongScene : Scene
    {
        private IAssetRepository _assetRepository;
        private InputHelper _inputHelper;
        private IComponentRepository _componentRepository;

        private RenderSystem _renderSystem;
        private InputSystem _inputSystem;
        private MotionSystem _motionSystem;
        private CollisionSystem _collisionSystem;
        private GraphicsHelper _graphicsHelper;

        private float _scale;
        private float scoreDigitOffsetX = 3f;
        private int p1Score;
        private int p2Score;

        public PongScene(IAssetRepository assetRepository, IComponentRepository componentRepository, InputHelper inputHelper, GraphicsHelper graphicsHelper)
        {
            _assetRepository = assetRepository;
            _componentRepository = componentRepository;
            _inputHelper = inputHelper;
            _graphicsHelper = graphicsHelper;
            _scale = 1f; //2f;
        }

        protected override void OnLoading()
        {
            p1Score = 0;
            p2Score = 0;
            var ballTexture = _assetRepository.GetTexture("Pong/Ball");
            var netTexture = _assetRepository.GetTexture("Pong/Net");
            var score0Texture = _assetRepository.GetTexture("Pong/Score_0");
            var paddleTexture = _assetRepository.GetTexture("Pong/Paddle");

            #region Initialise Systems
            _renderSystem = new RenderSystem(_componentRepository);
            _inputSystem = new InputSystem(_componentRepository, _inputHelper, _graphicsHelper);
            _motionSystem = new MotionSystem(_componentRepository);
            _collisionSystem = new CollisionSystem(_componentRepository);
            #endregion

            #region Initialise Net
            var netEntity = new Entity() 
            { 
                Id = Guid.NewGuid() 
            };
            var netTransformComponent = new TransformComponent()
            {
                Position = new Vector2(174 * _scale, 17 * _scale)
            };

            var netImageComponent = new ImageComponent(netTexture);

            _componentRepository.SetComponent(netEntity.Id, netTransformComponent);
            _componentRepository.SetComponent(netEntity.Id, netImageComponent);

            _renderSystem.Register(netEntity.Id);
            #endregion


            #region Initialise P1 Score
            var p1ScoreEntity = new Entity()
            {
                Id = Guid.NewGuid()
            };
            var p1ScoreTransformComponent = new TransformComponent()
            {
                Position = new Vector2(114 * _scale, 34 * _scale) - new Vector2(_assetRepository.GetTexture("Pong/Score_0").Width * _scale, 0)
            };
            var p1ScoreImageComponent = new ImageComponent(score0Texture);

            _componentRepository.SetComponent(p1ScoreEntity.Id, p1ScoreTransformComponent);
            _componentRepository.SetComponent(p1ScoreEntity.Id, p1ScoreImageComponent);

            _renderSystem.Register(p1ScoreEntity.Id);
            #endregion

            #region Initialise P2 Score
            var p2ScoreEntity = new Entity()
            {
                Id = Guid.NewGuid()
            };
            var p2ScoreTransformComponent = new TransformComponent()
            {
                Position = new Vector2(296 * _scale, 34 * _scale) - new Vector2(_assetRepository.GetTexture("Pong/Score_0").Width * _scale, 0)
            };
            var p2ScoreImageComponent = new ImageComponent(score0Texture);

            _componentRepository.SetComponent(p2ScoreEntity.Id, p2ScoreTransformComponent);
            _componentRepository.SetComponent(p2ScoreEntity.Id, p2ScoreImageComponent);

            _renderSystem.Register(p2ScoreEntity.Id);
            #endregion

            #region Initialise P1 Paddle
            var p1PaddleEntity = new Entity()
            {
                Id = Guid.NewGuid()
            };
            var p1PaddleTransformComponent = new TransformComponent()
            {
                Position = new Vector2(52 * _scale, 0)
            };
            var p1PaddleMeshComponent = new MeshComponent()
            {
                Vertices = new List<Vector2>()
                {
                    new Vector2(4, 0),
                    new Vector2(4, 14)
                },
                Edges = new List<Tuple<int, int>>()
                {
                    new Tuple<int, int>(0, 1)
                }
            };
            var p1PaddleImageComponent = new ImageComponent(paddleTexture);
            var p1InputComponent = new InputComponent()
            {
                OnMouseMove = (x, y) =>
                {
                    var maxY = _graphicsHelper.GetWindowBounds().Height - (p1PaddleImageComponent.Rectangle.Height * _scale);
                    y = y > maxY ? maxY : y;

                    p1PaddleTransformComponent.Position = new Vector2(p1PaddleTransformComponent.Position.X, y);
                }
            };

            _componentRepository.SetComponent(p1PaddleEntity.Id, p1PaddleTransformComponent);
            _componentRepository.SetComponent(p1PaddleEntity.Id, p1PaddleMeshComponent);
            _componentRepository.SetComponent(p1PaddleEntity.Id, p1PaddleImageComponent);
            _componentRepository.SetComponent(p1PaddleEntity.Id, p1InputComponent);


            //_collisionSystem.Register(p1PaddleEntity.Id);
            _renderSystem.Register(p1PaddleEntity.Id);
            _inputSystem.Register(p1PaddleEntity.Id);
            #endregion

            #region Initialise P2 Paddle
            var p2PaddleEntity = new Entity()
            {
                Id = Guid.NewGuid()
            };
            var p2PaddleTransformComponent = new TransformComponent()
            {
                Position = new Vector2(298 * _scale, 65)
            };
            var p2PaddleImageComponent = new ImageComponent(paddleTexture);

            _componentRepository.SetComponent(p2PaddleEntity.Id, p2PaddleTransformComponent);
            _componentRepository.SetComponent(p2PaddleEntity.Id, p2PaddleImageComponent);

            _renderSystem.Register(p2PaddleEntity.Id);
            #endregion

            #region Initialise Top Wall
            var topWallEntity = new Entity()
            {
                Id = Guid.NewGuid()
            };

            var topWallRigidBodyComponent = new RigidBodyComponent()
            {
                Mass = float.PositiveInfinity
            };
            var topWallMeshComponent = new MeshComponent()
            {
                Vertices = new List<Vector2>()
                {
                    new Vector2(0, 0),
                    new Vector2(370, 0)
                },
                Edges = new List<Tuple<int, int>>()
                {
                    new Tuple<int, int>(0, 1)
                }
            };
            var topWallTransformComponent = new TransformComponent()
            {
                Position = new Vector2(0, 0)
            };
            var topWallMotionComponent = new MotionComponent()
            {
                Velocity = Vector2.Zero,
                Acceleration = Vector2.Zero
            };

            _componentRepository.SetComponent(topWallEntity.Id, topWallRigidBodyComponent);
            _componentRepository.SetComponent(topWallEntity.Id, topWallMeshComponent);
            _componentRepository.SetComponent(topWallEntity.Id, topWallMotionComponent);
            _componentRepository.SetComponent(topWallEntity.Id, topWallTransformComponent);
            _componentRepository.SetComponent(topWallEntity.Id, new EventComponent()
            {
                OnCollision = (ce) => { }
            });
            #endregion


            #region Initialise Bottom Wall
            var bottomWallEntity = new Entity()
            {
                Id = Guid.NewGuid()
            };

            var bottomWallRigidBodyComponent = new RigidBodyComponent()
            {
                Mass = float.PositiveInfinity
            };
            var bottomWallMeshComponent = new MeshComponent()
            {
                Vertices = new List<Vector2>()
                {
                    new Vector2(0, 0),
                    new Vector2(370, 0)
                },
                Edges = new List<Tuple<int, int>>()
                {
                    new Tuple<int, int>(0, 1)
                }
            };
            var bottomWallTransformComponent = new TransformComponent()
            {
                Position = new Vector2(0, 290)
            };
            var bottomWallMotionComponent = new MotionComponent()
            {
                Velocity = Vector2.Zero,
                Acceleration = Vector2.Zero
            };

            _componentRepository.SetComponent(bottomWallEntity.Id, bottomWallRigidBodyComponent);
            _componentRepository.SetComponent(bottomWallEntity.Id, bottomWallMeshComponent);
            _componentRepository.SetComponent(bottomWallEntity.Id, bottomWallMotionComponent);
            _componentRepository.SetComponent(bottomWallEntity.Id, bottomWallTransformComponent);
            _componentRepository.SetComponent(bottomWallEntity.Id, new EventComponent()
            {
                OnCollision = (ce) => { }
            });
            #endregion


            #region Initialise Left Wall
            var leftWallEntity = new Entity()
            {
                Id = Guid.NewGuid()
            };

            var leftWallRigidBodyComponent = new RigidBodyComponent()
            {
                Mass = float.PositiveInfinity
            };
            var leftWallMeshComponent = new MeshComponent()
            {
                Vertices = new List<Vector2>()
                {
                    new Vector2(0, 0),
                    new Vector2(0, 290)
                },
                Edges = new List<Tuple<int, int>>()
                {
                    new Tuple<int, int>(0, 1)
                }
            };
            var leftWallTransformComponent = new TransformComponent()
            {
                Position = new Vector2(0, 0)
            };
            var leftWallMotionComponent = new MotionComponent()
            {
                Velocity = Vector2.Zero,
                Acceleration = Vector2.Zero
            };

            _componentRepository.SetComponent(leftWallEntity.Id, leftWallRigidBodyComponent);
            _componentRepository.SetComponent(leftWallEntity.Id, leftWallMeshComponent);
            _componentRepository.SetComponent(leftWallEntity.Id, leftWallMotionComponent);
            _componentRepository.SetComponent(leftWallEntity.Id, leftWallTransformComponent);
            _componentRepository.SetComponent(leftWallEntity.Id, new EventComponent()
            {
                OnCollision = (ce) => { }
            });
            #endregion

            #region Initialise Right Wall
            var rightWallEntity = new Entity()
            {
                Id = Guid.NewGuid()
            };

            var rightWallRigidBodyComponent = new RigidBodyComponent()
            {
                Mass = float.PositiveInfinity
            };
            var rightWallMeshComponent = new MeshComponent()
            {
                Vertices = new List<Vector2>()
                {
                    new Vector2(0, 0),
                    new Vector2(0, 290)
                },
                Edges = new List<Tuple<int, int>>()
                {
                    new Tuple<int, int>(0, 1)
                }
            };
            var rightWallTransformComponent = new TransformComponent()
            {
                Position = new Vector2(370, 0)
            };
            var rightWallMotionComponent = new MotionComponent()
            {
                Velocity = Vector2.Zero,
                Acceleration = Vector2.Zero
            };

            _componentRepository.SetComponent(rightWallEntity.Id, rightWallRigidBodyComponent);
            _componentRepository.SetComponent(rightWallEntity.Id, rightWallMeshComponent);
            _componentRepository.SetComponent(rightWallEntity.Id, rightWallMotionComponent);
            _componentRepository.SetComponent(rightWallEntity.Id, rightWallTransformComponent);
            _componentRepository.SetComponent(rightWallEntity.Id, new EventComponent()
            {
                OnCollision = (ce) => { }
            });

            #endregion

            #region Initialise Ball
            var ballEntity = new Entity()
            {
                Id = Guid.NewGuid()
            };
            // Model the ball as a point mass
            var ballRigidBodyComponent = new RigidBodyComponent()
            {
                Mass = 1f
            };
            var ballMeshComponent = new MeshComponent()
            {
                Vertices = new List<Vector2>()
                {
                    Vector2.Zero
                }
            };
            Random rnd = new Random();

            var radians = rnd.NextDouble() * 2 * Math.PI;
            var speed = 100;
            var velocity = new Vector2((float)Math.Cos(radians) * speed, -(float)Math.Sin(radians) * speed);

            var ballTransformComponent = new TransformComponent()
            {
                Position = new Vector2(174 * _scale, 100f)
            };
            var ballMotionComponent = new MotionComponent()
            {
                Velocity = velocity
            };
            var ballImageComponent = new ImageComponent(ballTexture);

            _componentRepository.SetComponent(ballEntity.Id, ballRigidBodyComponent);
            _componentRepository.SetComponent(ballEntity.Id, ballMeshComponent);
            _componentRepository.SetComponent(ballEntity.Id, ballTransformComponent);
            _componentRepository.SetComponent(ballEntity.Id, ballMotionComponent);
            _componentRepository.SetComponent(ballEntity.Id, ballImageComponent);
            _componentRepository.SetComponent(ballEntity.Id, new EventComponent()
            {
                OnCollision = (ce) =>
                {
                    if (ce.EntityIds.Item2 == leftWallEntity.Id)
                    {
                        p2Score += 1;

                        var newScoreTexture = _assetRepository.GetTexture($"Pong/Score_{p2Score}");
                        var p2ScoreTransformComponent = new TransformComponent()
                        {
                            Position = new Vector2(296 * _scale, 34 * _scale) - new Vector2(newScoreTexture.Width * _scale, 0)
                        };
                        var p2ScoreImageComponent = new ImageComponent(newScoreTexture);

                        _componentRepository.SetComponent(p2ScoreEntity.Id, p2ScoreImageComponent);
                        _componentRepository.SetComponent(p2ScoreEntity.Id, p2ScoreTransformComponent);
                    }

                    if (ce.EntityIds.Item2 == rightWallEntity.Id)
                    {
                        p1Score += 1;

                        var newScoreTexture = _assetRepository.GetTexture($"Pong/Score_{p1Score}");
                        var p1ScoreTransformComponent = new TransformComponent()
                        {
                            Position = new Vector2(114 * _scale, 34 * _scale) - new Vector2(newScoreTexture.Width * _scale, 0)
                        };
                        var p1ScoreImageComponent = new ImageComponent(newScoreTexture);

                        _componentRepository.SetComponent(p1ScoreEntity.Id, p1ScoreImageComponent);
                        _componentRepository.SetComponent(p1ScoreEntity.Id, p1ScoreTransformComponent);
                    }
                }
            });

            _renderSystem.Register(ballEntity.Id);
            _motionSystem.Register(ballEntity.Id);


            // Ball needs to be first for now due to ordering in CollisionSystem WIP
            _collisionSystem.Register(ballEntity.Id);
            _collisionSystem.Register(topWallEntity.Id);
            _collisionSystem.Register(bottomWallEntity.Id);
            _collisionSystem.Register(leftWallEntity.Id);
            _collisionSystem.Register(rightWallEntity.Id);
            #endregion
        }

        public override void Update(GameTime gameTime)
        {
            _inputSystem.Update(gameTime);
            _collisionSystem.Update(gameTime);
            _motionSystem.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, samplerState: SamplerState.PointWrap);
            _renderSystem.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
