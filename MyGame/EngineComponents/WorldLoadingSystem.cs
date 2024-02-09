using Microsoft.Xna.Framework;
using Project1.Engine;
using Project1.Engine.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.MyGame
{
    internal class WorldLoadingSystem : SystemComponent
    {

        public Vector3 PlayerLocation { get; private set; }
        public Vector3 KillLevel { get; private set; }
        public int HoleId { get; private set; }

        private World _world;
        public WorldLoadingSystem(World world)
        {
            _world = world;
        }

        public void LoadWorld(string path)
        {
            string content = File.ReadAllText(Path.Combine(_world.Game.Content.RootDirectory, "..", path));
            Console.WriteLine($"Loading world {Path.GetFileName(path)}");
            string[] lines = content.Split("\n");
            Console.WriteLine($" | Loading {lines.Length} empties");
            foreach(var x in lines)
            {
                if (x.Trim().Length != 0)
                    LoadEmpty(x);
            }
        }

        private void LoadEmpty(string line)
        {
            string[] args = line.Split(':');
            string name = args[0];
            Vector3 pos = new Vector3(float.Parse(args[1]), float.Parse(args[3]), float.Parse(args[2]));
            Vector3 scale = new Vector3(float.Parse(args[4]), float.Parse(args[6]), float.Parse(args[5]));
            Vector3 rotation = -new Vector3(float.Parse(args[7]), float.Parse(args[9]), float.Parse(args[8]));

            Matrix localMatrix = Matrix.CreateScale(scale * 2) *
                            Matrix.CreateRotationZ(MathHelper.ToRadians(rotation.Z)) *
                            Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y)) *
                            Matrix.CreateRotationX(MathHelper.ToRadians(rotation.X));
            Matrix worldMatrix = localMatrix * Matrix.CreateTranslation(pos);

            switch (name.ToLower())
            {
                case "hole":
                    var hole = _world.CreateEntity()
                        .AddComponent(new PositionComponent(worldMatrix))
                        .AddComponent(new PrimitivePhysicsComponent(RigidBodyType.Box, BulletSharp.CollisionFilterGroups.SensorTrigger));
                    HoleId = hole.Id;
                    break;
                case "box":
                    _world.CreateEntity()
                        .AddComponent(new PositionComponent(worldMatrix))
                        .AddComponent(new MeshComponent("Models/Cube"))
                        .AddComponent(new PrimitivePhysicsComponent(RigidBodyType.Box));
                    break;
                case "sphere":
                    // dunno, just dont want spheres
                    // phyx = new PrimitivePhysicsComponent(RigidBody.Sphere, RigidBodyFlags.Static, scale.X);
                    // _world.CreateEntity()
                    //     .AddComponent(new PositionComponent(worldMatrix))
                    //     //.AddComponent(new MeshComponent("Models/DebugSphere", "Shaders/GroundShader"))
                    //     .AddComponent(phyx);
                    break;
                case "player":
                    PlayerLocation = pos;
                    break;
                case "kill":
                    KillLevel = pos;
                    break;
            }
        }

    }
}
