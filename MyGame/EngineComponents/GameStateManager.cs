using Microsoft.Xna.Framework;
using Project1.Engine;
using Project1.Engine.Components;
using Project1.Engine.Systems;
using Project1.Engine.Systems.GUI;
using Project1.Engine.Systems.RenderMessages;
using Project1.MyGame;
using Project2.Engine;
using Project2.Engine.Components;
using Project2.MyGame.GUIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2.MyGame.EngineComponents
{
    internal class GameStateManager : SystemComponent
    {

        public int CompletedRings { get; private set; }
        public int TotalRings { get; private set; }
        public int Points { get; private set; }
        public uint WorldSeed { get; private set; }
        public float ElapsedTime { get; private set; }

        private Vector4[] _rings;

        private int _playerId;
        private PhysicsSystem _physics;
        private WorldGenerationSystem _worldGen;
        private World _world;
        private Camera _camera;
        private DateTime _start;

        public Action Exit;

        public GameStateManager(Camera cam, World w, PhysicsSystem physics, WorldGenerationSystem worldGen, HudSystem hud)
        {
            this._camera = cam;
            this._world = w;
            this._worldGen = worldGen;
            this._physics = physics;
            _physics.Collision += Collision;

            Render.EnqueueMessage(new RenderMessageLoadTexture("Textures/GUI/GPS"));
            Render.EnqueueMessage(new RenderMessageLoadTexture("Textures/GUI/arrow"));
        }

        public void CreateWorld(byte checkpoints, byte difficulty, ushort seed)
        {
            _worldGen.CreateRandomWorld(checkpoints, difficulty, seed);
            WorldSeed = WorldGenerationSystem.CreateSeed(checkpoints, difficulty, seed);
            TotalRings = checkpoints - 1;
            CompletedRings = 1;
            _rings = _worldGen.Checkpoints;
            _start = DateTime.Now;

            var hud = _world.GetSystem<HudSystem>();
            var spaceship = _world.CreateEntity()
                .AddComponent(new PositionComponent(Matrix.Identity))
                .AddComponent(new PrimitivePhysicsComponent(RigidBodyType.Sphere, 10))
                .AddComponent(new SpaceshipController(Matrix.CreateTranslation(new Vector3(0, 0.4f, 0.7f))))
                .AddComponent(new MeshComponent("Models/Cockpit", "Textures/Spaceship/CT", "Textures/Spaceship/ADD"))
                .AddComponent(new MeshRenderingComponent());
            hud.RegisterElement(new HudSpeed(spaceship, new Vector2I(256, 256), "info"));
            hud.RegisterElement(new HudInfo(this, new Vector2I(256, 256), "dashboard"));
            hud.RegisterElement(new HudHealth(spaceship, new Vector2I(256, 256), "health"));

            _playerId = spaceship.Id;
        }

        private void Collision(int ent, int with, Vector3 pos, Vector3 normal, float impulse)
        {
            if (CompletedRings <= TotalRings && with == _playerId && ent == (int)_rings[CompletedRings].W)
            {
                int speedPoints = (int)_world.GetEntity(_playerId).GetComponent<PrimitivePhysicsComponent>().LinearVelocity.LengthSquared();
                Points += 100 + Math.Min(speedPoints, 5000);
                CompletedRings++;
            }
        }

        public override void Update(GameTime delta)
        {
            if (_world.GetEntity(_playerId).GetComponent<SpaceshipController>().Health <= 0)
            {
                Exit?.Invoke();
                return;
            }
            if (CompletedRings == TotalRings + 1)
            {
                GameSaverLoader.SaveGame(new GameSaverLoader.Save
                {
                    LevelData = WorldSeed,
                    Points = Points,
                    Time = TimeSpan.FromSeconds(ElapsedTime).Ticks,
                });
                Exit?.Invoke();
                return;
            }

            if (CompletedRings > TotalRings)
                return;

            ElapsedTime = (float)(DateTime.Now - _start).TotalSeconds;

            Vector4 cpos = _rings[CompletedRings];
            Vector3 wpos = new Vector3(cpos.X, cpos.Y, cpos.Z);

            if (Vector3.Dot(_camera.Forward, _camera.Translation - wpos) > .5f)
                return;

            Vector3 screen = _camera.WorldToScreen(wpos);

            Rectangle rect = new Rectangle((int)screen.X - 10, (int)screen.Y - 10, 20, 20);
            Render.EnqueueMessage(new RenderMessageDrawColoredSprite("Textures/GUI/GPS", rect, 1, Color.CadetBlue));
        }

        public override void Close()
        {
            _physics.Collision -= Collision;
        }

    }
}
