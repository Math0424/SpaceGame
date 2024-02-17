using Microsoft.Xna.Framework;
using Project1.Engine;
using Project1.Engine.Components;
using Project1.Engine.Systems;
using Project1.Engine.Systems.GUI;
using Project1.MyGame;
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
        public ulong WorldSeed { get; private set; }

        private Vector4[] _rings;

        private int _playerId;
        private PhysicsSystem _physics;
        private WorldGenerationSystem _worldGen;
        private World _world;

        public GameStateManager(World w, PhysicsSystem physics, WorldGenerationSystem worldGen, HudSystem hud)
        {
            this._world = w;
            this._worldGen = worldGen;
            this._physics = physics;
            _physics.Collision += Collision;
        }

        public void CreateWorld(byte checkpoints, float difficulty)
        {
            Random r = new Random();
            ushort seed = (ushort)r.Next(ushort.MaxValue);
            _worldGen.CreateRandomWorld(checkpoints, difficulty, seed);
            WorldSeed = WorldGenerationSystem.CreateSeed(checkpoints, difficulty, seed);
            TotalRings = checkpoints;
            CompletedRings = 1;
            _rings = _worldGen.Checkpoints;
        }

        private void Collision(int ent, int with, Vector3 pos, Vector3 normal, float impulse)
        {
            if (CompletedRings < TotalRings && ent == _playerId && with == (int)_rings[CompletedRings].W)
            {
                int speedPoints = (int)_world.GetEntity(_playerId).GetComponent<PrimitivePhysicsComponent>().LinearVelocity.LengthSquared();
                Points += 100 + Math.Min(speedPoints, 5000);
                CompletedRings++;
            }
        }

        public override void Update(GameTime delta)
        {

        }

        public override void Close()
        {
            _physics.Collision -= Collision;
        }

    }
}
