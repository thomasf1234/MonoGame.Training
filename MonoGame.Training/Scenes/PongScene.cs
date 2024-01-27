using Microsoft.Xna.Framework;
using MonoGame.Training.Repositories;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Training.Helpers;
using MonoGame.Training.Components;
using MonoGame.Training.Entities;
using System;
using MonoGame.Training.Systems;
using System.Collections.Generic;
using MonoGame.Training.Models;
using MonoGame.Training.Models.Geometry;

// https://badecho.com/index.php/2023/08/02/alpha-spritebatch/
namespace MonoGame.Training.Scenes
{
    public class PongScene : Scene
    {
        private RenderTarget2D _nativeRenderTarget;
        private IAssetRepository _assetRepository;
        private InputHelper _inputHelper;
        private IComponentRepository _componentRepository;

        private SpriteRenderSystem _renderSystem;
        private InputSystem _inputSystem;
        private PhysicsSystem _physicsSystem;
        private CollisionSystem _collisionSystem;
        private MetricSystem _metricSystem;
        private TextRenderSystem _textRenderSystem;

        private GraphicsHelper _graphicsHelper;

        private Rectangle _actualScreenRectangle;

        private float _scale;
        private float scoreDigitOffsetX = 3f;
        private int p1Score;
        private int p2Score;

        public PongScene(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, IAssetRepository assetRepository, IComponentRepository componentRepository, InputHelper inputHelper, GraphicsHelper graphicsHelper) : base(spriteBatch, graphicsDevice)
        {
            _assetRepository = assetRepository;
            _componentRepository = componentRepository;
            _inputHelper = inputHelper;
            _graphicsHelper = graphicsHelper;
            _scale = 2f;
        }

        protected override void OnLoading()
        {
            // Initialization
            _actualScreenRectangle = new Rectangle(0, 0, (int)(370 * _scale), (int)(290 * _scale));
            var nativeScreenRectangle = new Rectangle(0, 0, 370, 290);
            _nativeRenderTarget = new RenderTarget2D(_graphicsHelper.GetGraphicsDeviceManager().GraphicsDevice, nativeScreenRectangle.Width, nativeScreenRectangle.Height);

            p1Score = 0;
            p2Score = 0;
            var ballTexture = _assetRepository.GetTexture("Pong/Ball");
            var netTexture = _assetRepository.GetTexture("Pong/Net");
            var score0Texture = _assetRepository.GetTexture("Pong/Score_0");
            var paddleTexture = _assetRepository.GetTexture("Pong/Paddle");

            #region Initialise Systems
            _renderSystem = new SpriteRenderSystem(_componentRepository, SpriteBatch);
            _inputSystem = new InputSystem(_componentRepository, _inputHelper, _graphicsHelper);
            _physicsSystem = new PhysicsSystem(_componentRepository);
            _collisionSystem = new CollisionSystem(_componentRepository);
            _metricSystem = new MetricSystem(_componentRepository);
            _textRenderSystem = new TextRenderSystem(_componentRepository, _assetRepository);   
            #endregion

            #region Initialise Metrics Summary
            var metricsEntity = new Entity() { Id = Guid.NewGuid() };

            var metricsTextComponent = new TextComponent()
            {
                FontId = "Arial",
                Color = Color.White,
                Opacity = 1f,
                Text = ""
            };

            var metricsTransformComponent = new TransformComponent()
            {
                Position = new Vector2(10, 10)
            };

            _componentRepository.SetComponent(metricsEntity.Id, metricsTextComponent);
            _componentRepository.SetComponent(metricsEntity.Id, metricsTransformComponent);

            _metricSystem.Register(new List<Guid>() { metricsEntity.Id });
            _textRenderSystem.Register(new List<Guid>() { metricsEntity.Id });
            #endregion

            #region Initialise Net
            var netEntity = new Entity() 
            { 
                Id = Guid.NewGuid() 
            };
            var netTransformComponent = new TransformComponent()
            {
                Position = new Vector2(174, 17)
            };

            var netTextureComponent = new TextureComponent(netTexture);

            _componentRepository.SetComponent(netEntity.Id, netTransformComponent);
            _componentRepository.SetComponent(netEntity.Id, netTextureComponent);

            _renderSystem.Register(netEntity.Id);
            #endregion


            #region Initialise P1 Score
            var p1ScoreEntity = new Entity()
            {
                Id = Guid.NewGuid()
            };
            var p1ScoreTransformComponent = new TransformComponent()
            {
                Position = new Vector2(114, 34) - new Vector2(_assetRepository.GetTexture("Pong/Score_0").Width, 0)
            };
            var p1ScoreTextureComponent = new TextureComponent(score0Texture);

            _componentRepository.SetComponent(p1ScoreEntity.Id, p1ScoreTransformComponent);
            _componentRepository.SetComponent(p1ScoreEntity.Id, p1ScoreTextureComponent);

            _renderSystem.Register(p1ScoreEntity.Id);
            #endregion

            #region Initialise P2 Score
            var p2ScoreEntity = new Entity()
            {
                Id = Guid.NewGuid()
            };
            var p2ScoreTransformComponent = new TransformComponent()
            {
                Position = new Vector2(296, 34) - new Vector2(_assetRepository.GetTexture("Pong/Score_0").Width, 0)
            };
            var p2ScoreTextureComponent = new TextureComponent(score0Texture);

            _componentRepository.SetComponent(p2ScoreEntity.Id, p2ScoreTransformComponent);
            _componentRepository.SetComponent(p2ScoreEntity.Id, p2ScoreTextureComponent);

            _renderSystem.Register(p2ScoreEntity.Id);
            #endregion

            #region Initialise P1 Paddle
            var p1PaddleEntity = new Entity()
            {
                Id = Guid.NewGuid()
            };
            var p1PaddleRigidBodyComponent = new RigidBodyComponent()
            {
                Mass = float.PositiveInfinity
            };
            var p1PaddleTransformComponent = new TransformComponent()
            {
                Position = new Vector2(52, 0)
            };
            var p1PaddleMotionComponent = new MotionComponent()
            {
                Velocity = Vector2.Zero,
                Acceleration = Vector2.Zero
            };
            var p1PaddleMeshComponent = new MeshComponent()
            {
                Vertices = new List<Vector2>()
                {
                    new Vector2(4, 0),
                    new Vector2(4, 14)

                },
                Edges = new List<Edge>()
                {
                    new Edge(0, 1)
                }
            };
            var p1PaddleTextureComponent = new TextureComponent(paddleTexture);
            var p1InputComponent = new InputComponent()
            {
                OnMouseMove = (x, y) =>
                {
                    var maxY = (_graphicsHelper.GetWindowBounds().Height / _scale) - (p1PaddleTextureComponent.Rectangle.Height);

                    x = x / _scale;
                    y = y / _scale;
                    y = y > maxY ? maxY : y;

                    p1PaddleTransformComponent.Position = new Vector2(p1PaddleTransformComponent.Position.X, y);
                }
            };

            _componentRepository.SetComponent(p1PaddleEntity.Id, p1PaddleRigidBodyComponent);
            _componentRepository.SetComponent(p1PaddleEntity.Id, p1PaddleTransformComponent);
            _componentRepository.SetComponent(p1PaddleEntity.Id, p1PaddleMotionComponent);    
            _componentRepository.SetComponent(p1PaddleEntity.Id, p1PaddleMeshComponent);
            _componentRepository.SetComponent(p1PaddleEntity.Id, p1PaddleTextureComponent);
            _componentRepository.SetComponent(p1PaddleEntity.Id, p1InputComponent);
            _componentRepository.SetComponent(p1PaddleEntity.Id, new ImpulseComponent()
            {
                Impulses = new List<Impulse>()
                {
                    new Impulse()
                    {

                    }
                }
            });
            _componentRepository.SetComponent(p1PaddleEntity.Id, new EventComponent()
            {
                OnCollision = (ce) => { }
            });


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
                Position = new Vector2(298, 65)
            };
            var p2PaddleTextureComponent = new TextureComponent(paddleTexture);

            _componentRepository.SetComponent(p2PaddleEntity.Id, p2PaddleTransformComponent);
            _componentRepository.SetComponent(p2PaddleEntity.Id, p2PaddleTextureComponent);

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
                Edges = new List<Edge>()
                {
                    new Edge(0, 1)
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
            _componentRepository.SetComponent(topWallEntity.Id, new ImpulseComponent()
            {
                Impulses = new List<Impulse>()
                {
                    new Impulse()
                    {

                    }
                }
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
                Edges = new List<Edge>()
                {
                    new Edge(0, 1)
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
            _componentRepository.SetComponent(bottomWallEntity.Id, new ImpulseComponent()
            {
                Impulses = new List<Impulse>()
                {
                    new Impulse()
                    {

                    }
                }
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
                Edges = new List<Edge>()
                {
                    new Edge(0, 1)
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
                OnCollision = (ce) => 
                {
                    p2Score += 1;

                    var newScoreTexture = _assetRepository.GetTexture($"Pong/Score_{p2Score}");
                    var p2ScoreTransformComponent = new TransformComponent()
                    {
                        Position = new Vector2(296, 34) - new Vector2(newScoreTexture.Width, 0)
                    };
                    var p2ScoreImageComponent = new TextureComponent(newScoreTexture);

                    _componentRepository.SetComponent(p2ScoreEntity.Id, p2ScoreImageComponent);
                    _componentRepository.SetComponent(p2ScoreEntity.Id, p2ScoreTransformComponent);
                }
            });
            _componentRepository.SetComponent(leftWallEntity.Id, new ImpulseComponent()
            {
                Impulses = new List<Impulse>()
                {
                    new Impulse()
                    {

                    }
                }
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
                Edges = new List<Edge>()
                {
                    new Edge(0, 1)
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
                OnCollision = (ce) => 
                {
                    p1Score += 1;

                    var newScoreTexture = _assetRepository.GetTexture($"Pong/Score_{p1Score}");
                    var p1ScoreTransformComponent = new TransformComponent()
                    {
                        Position = new Vector2(114, 34) - new Vector2(newScoreTexture.Width, 0)
                    };
                    var p1ScoreImageComponent = new TextureComponent(newScoreTexture);

                    _componentRepository.SetComponent(p1ScoreEntity.Id, p1ScoreImageComponent);
                    _componentRepository.SetComponent(p1ScoreEntity.Id, p1ScoreTransformComponent);
                }
            });
            _componentRepository.SetComponent(rightWallEntity.Id, new ImpulseComponent()
            {
                Impulses = new List<Impulse>()
                {
                    new Impulse()
                    {

                    }
                }
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

            var ballTransformComponent = new TransformComponent()
            {
                Position = new Vector2(174, 100f)
            };
            var ballMotionComponent = new MotionComponent()
            {
                Velocity = RandomBallVelocity()
            };
            var ballTextureComponent = new TextureComponent(ballTexture);

            _componentRepository.SetComponent(ballEntity.Id, ballRigidBodyComponent);
            _componentRepository.SetComponent(ballEntity.Id, ballMeshComponent);
            _componentRepository.SetComponent(ballEntity.Id, ballTransformComponent);
            _componentRepository.SetComponent(ballEntity.Id, ballMotionComponent);
            _componentRepository.SetComponent(ballEntity.Id, ballTextureComponent);
            _componentRepository.SetComponent(ballEntity.Id, new ImpulseComponent()
            {
                Impulses = new List<Impulse>()
                {
                    new Impulse()
                    {
 
                    }
                }
            });
            _componentRepository.SetComponent(ballEntity.Id, new EventComponent()
            {
                OnCollision = (ce) => { }
            });

            _renderSystem.Register(ballEntity.Id);
            _physicsSystem.Register(ballEntity.Id);

            // Ball needs to be first for now due to ordering in CollisionSystem WIP
            _collisionSystem.Register(ballEntity.Id);
            _collisionSystem.Register(topWallEntity.Id);
            _collisionSystem.Register(bottomWallEntity.Id);
            _collisionSystem.Register(leftWallEntity.Id);
            _collisionSystem.Register(rightWallEntity.Id);
            _collisionSystem.Register(p1PaddleEntity.Id);
            #endregion
        }

        public override void Update(GameTime gameTime)
        {
            _metricSystem.Update(gameTime);
            _collisionSystem.Update(gameTime);
            _physicsSystem.Update(gameTime);

            // Process events
            // TODO : Process events

            // Input processed at the end of Update to ensure history has evolved correctly
            _inputSystem.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw call
            GraphicsDevice.SetRenderTarget(_nativeRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            // now render your game like you normally would, but if you change the render target somewhere,
            // make sure you set it back to this one and not the backbuffer
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, samplerState: SamplerState.PointWrap);
            _renderSystem.Draw(gameTime);
            _textRenderSystem.Draw(SpriteBatch);
            SpriteBatch.End();

            // after drawing the game at native resolution we can render _nativeRenderTarget to the backbuffer!
            // First set the GraphicsDevice target back to the backbuffer
            GraphicsDevice.SetRenderTarget(null);
            // RenderTarget2D inherits from Texture2D so we can render it just like a texture
            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            SpriteBatch.Draw(_nativeRenderTarget, _actualScreenRectangle, Color.White);
            SpriteBatch.End();    
        }

        private Vector2 RandomBallVelocity()
        {
            var rnd = new Random();
            var radians = rnd.NextDouble() * 0.5 * Math.PI;
            var speed = 200;
            var velocity = new Vector2((float)Math.Cos(radians) * speed, -(float)Math.Sin(radians) * speed);

            return velocity;
        }
    }
}
