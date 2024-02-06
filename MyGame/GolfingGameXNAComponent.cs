#define SPECTATORs

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Project1.Engine;
using Project1.Engine.Components;
using Project1.Engine.Systems;
using Project1.Engine.Systems.GUI;
using Project1.Engine.Systems.RenderMessages;
using Project1.MyGame.EngineComponents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.MyGame
{
    internal class GolfingGameXNAComponent : GameComponent
    {
        public Action InvokeMainMenu;

        private Game _game;
        private int _playerCount;
        private World _world;
        private string[] _worlds;
        private int _currentWorld;

        private Entity[] _players;
        private int _currentPlayerToGolf;
        private int _playersRemain;

        private Entity _currentPlayer;
        private int _holeId;

        private GolfingGUI _gui;

        private Color[] _playerColors = new[]
        {
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Yellow,
            Color.Orange,
            Color.Pink,
            Color.Aqua,
            Color.Gold,
            Color.MistyRose,
        };

        public GolfingGameXNAComponent(Game game, string startingWorld, int playerCount) : base(game)
        {
            _game = game;
            _playerCount = playerCount;

            _world = new World(_game)
                .AddSystem<Camera>()
                .AddSystem<RenderingSystem>()
                .AddSystem<PhysicsSystem>()
                .AddSystem<SoundSystem>()
                .AddSystem<WorldLoadingSystem>()
#if SPECTATOR
                .AddSystem<SpectatorMovement>()
#endif
                .AddSystem<HudSystem>();
            _game.Components.Add(_world);

            _world.Render.EnqueueMessage(new RenderMessageLoadTexture("Textures/GUI/circle"));
            _worlds = Directory.GetFiles(Path.Combine(Game.Content.RootDirectory, "worlds"));

            if (startingWorld == null)
                _currentWorld = 0;
            else
            {
                for (int i = 0; i < _worlds.Length; i++)
                {
                    if (Path.GetFileNameWithoutExtension(_worlds[i]) == startingWorld)
                    {
                        _currentWorld = i;
                        break;
                    }
                }
            }
        }

        public override void Initialize()
        {
            var cam = _world.Render.Camera;
            cam.SetWorldMatrix(Matrix.CreateRotationX(-MathHelper.PiOver2));

            int aspectRatio = _world.Render.ScreenBounds.X / _world.Render.ScreenBounds.Y;
#if SPECTATOR
            cam.SetupProjection(_world.Render.ScreenBounds.X, _world.Render.ScreenBounds.Y, 100);
#else
            cam.SetupOrthographic(aspectRatio * 15, 15, -50f, 50f);
#endif

            var hud = _world.GetSystem<HudSystem>();
            _gui = new GolfingGUI(hud.Root);
            LoadWorld();
        }

        private void LoadWorld()
        {
            foreach (var e in _world.GetEntities())
                e.Close();

            var worldLoader = _world.GetSystem<WorldLoadingSystem>();
            worldLoader.LoadWorld(_worlds[_currentWorld]);
            _holeId = worldLoader.HoleId;

            var pysxSystem = _world.GetSystem<PhysicsSystem>();

            pysxSystem.Collision += HoleCollision;
            pysxSystem.Collision += ImpactSound;

            _players = new Entity[_playerCount];
            _playersRemain = _playerCount;
            for (int i = 0; i < _playerCount; i++)
            {
                _players[i] = _world.CreateEntity()
                    .AddComponent(new PositionComponent(Matrix.CreateTranslation(worldLoader.PlayerLocation)))
                    .AddComponent(new MeshComponent("models/sphere"))
#if !SPECTATOR
                    .AddComponent(new EntityGolfingComponent(worldLoader.PlayerLocation, worldLoader.KillLevel.Y, $"Player {i+1}", _playerColors[i % _playerColors.Length]))
#endif
                    .AddComponent(new PrimitivePhysicsComponent(RigidBodyType.Sphere, RigidBodyFlags.Dynamic, .08f, .5f));
                _players[i].Position.SetLocalMatrix(Matrix.CreateScale(.40f));
                var phyx = _players[i].GetComponent<PrimitivePhysicsComponent>();
                phyx.IsEnabled = false;
            }
            ActivateCurrentPlayer();
        }

        public void ImpactSound(int aId, int bId, Vector3 pos, Vector3 normal, float J)
        {
            if (J > .15f)
                _world.GetSystem<SoundSystem>().PlaySoundEffect("Audio/hit_wall");
        }


        private void ActivateCurrentPlayer()
        {
#if !SPECTATOR
            if (_playersRemain == 0)
                return;

            int add = 0;
            for(int i = _currentPlayerToGolf; i < _currentPlayerToGolf + _playerCount; i++)
            {
                if (_players[i % _playerCount].Id != -1)
                {
                    _currentPlayer = _players[i % _playerCount];
                    break;
                }
                add++;
            }
            var golf = _currentPlayer.GetComponent<EntityGolfingComponent>();
            golf.IsActive = true;
            _currentPlayer.GetComponent<PrimitivePhysicsComponent>().IsEnabled = true;

            _gui.WorldName = $"{Path.GetFileNameWithoutExtension(_worlds[_currentWorld])} - {golf.Name}";
            _gui.StrokeCount = $"{_currentPlayer.GetComponent<EntityGolfingComponent>().Strokes} Stroke(s)";
#endif
        }

        private void HoleCollision(int aId, int bId, Vector3 pos, Vector3 normal, float J)
        {
#if !SPECTATOR
            if (aId != _holeId) return;

            for(int i = 0; i < _playerCount; i++)
            {
                Entity player = _players[i];
                if (player.Id == bId)
                {
                    player.Close();
                    _playersRemain--;
                    if (_currentPlayer == player)
                    {
                        _currentPlayerToGolf++;
                        ActivateCurrentPlayer();
                        _world.GetSystem<SoundSystem>().PlaySoundEffect("Audio/enter_hole");
                    }
                    break;
                }
            }
            if (_playersRemain == 0)
            {
                _currentWorld++;
                if (_currentWorld == _worlds.Length)
                {
                    InvokeMainMenu?.Invoke();
                    return;
                }
                LoadWorld();
            }
#endif
        }

        public override void Update(GameTime gameTime)
        {
#if !SPECTATOR
            EntityGolfingComponent golfBall = _currentPlayer.GetComponent<EntityGolfingComponent>();
            if (golfBall != null && golfBall.TurnComplete)
            {
                golfBall.IsActive = false;
                _currentPlayerToGolf++;
                ActivateCurrentPlayer();
            }
#endif

            if (Input.IsNewKeyDown(Keys.Escape))
                InvokeMainMenu?.Invoke();
        }

        protected override void Dispose(bool disposing)
        {
            _game.Components.Remove(_world);
            _world.Dispose();
            base.Dispose(disposing);
        }

    }
}
