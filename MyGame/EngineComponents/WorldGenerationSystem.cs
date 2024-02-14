using Microsoft.Xna.Framework;
using Project1.Engine;
using Project1.Engine.Components;
using Project1.Engine.Systems.RenderMessages;
using Project2.Engine.Components;
using Project2.MyGame.EngineComponents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.MyGame
{
    internal class WorldGenerationSystem : SystemComponent
    {

        List<Vector3> _checkpoints;
        private World _world;
        public WorldGenerationSystem(World world)
        {
            _checkpoints = new List<Vector3>();
            _world = world;
        }

        public void CreateRandomWorld(float difficulty)
        {
            Random r = new Random();
            int numPoints = Math.Clamp(r.Next((int)(100 * difficulty)), 10, 70);
            Vector3[] randomPoints = new Vector3[numPoints];

            Console.WriteLine($"Creating a world with {numPoints} checkpoints");

            Vector3 dir = Vector3.Forward;
            Vector3 currPos = Vector3.Zero;
            float distanceBetween = 10;
            for(int i = 1; i < numPoints; i++)
            {
                currPos += dir * distanceBetween * difficulty;
                float offset = distanceBetween / (10 * (1 - difficulty) + .1f);
                currPos += new Vector3((float)r.NextDouble() * offset, (float)r.NextDouble() * offset, (float)r.NextDouble() * offset);
                randomPoints[i] = currPos;
                dir = Vector3.Normalize(randomPoints[i] - randomPoints[i - 1]);
            }

            _checkpoints.Add(Vector3.Zero);
            Vector3[] arrowPoints = MathExtensions.CatmullRom(randomPoints, .25f);
            for (int i = 1; i < arrowPoints.Length - 1; i++)
            {
                Vector3 normal = Vector3.Normalize(arrowPoints[i + 1] - arrowPoints[i - 1]);
                Vector3 pos = arrowPoints[i];

                if (i % 4 == 0)
                    SpawnCheckpoint(pos, normal);
                else
                {
                    _checkpoints.Add(pos);
                    Matrix transform = Matrix.CreateWorld(pos, normal, Vector3.Cross(pos, normal));
                    _world.CreateEntity()
                            .AddComponent(new PositionComponent(transform, Matrix.CreateScale(0.01f)))
                            .AddComponent(new MeshComponent("Models/Arrow"))
                            .AddComponent(new MeshRenderingComponent())
                            .AddComponent(new HideWhenClose());
                }
            }
        }

        private void SpawnCheckpoint(Vector3 pos, Vector3 normal)
        {
            _checkpoints.Add(pos);
            Matrix transform = Matrix.CreateWorld(pos, normal, Vector3.Cross(pos, normal));

            _world.CreateEntity()
                .AddComponent(new PositionComponent(transform, Matrix.CreateScale(0.01f)))
                .AddComponent(new MeshComponent("Models/ring"))
                .AddComponent(new MeshRenderingComponent());

            Hitbox[] checkpointBoxes = ReadFile("Hitboxes/Ring.txt");
            foreach(var x in checkpointBoxes)
            {
                if (x.Name.ToLower() == "box")
                {
                    Matrix translate = Matrix.CreateScale(x.Scale) * x.Rotation * Matrix.CreateTranslation(x.Position);
                    _world.CreateEntity()
                        .AddComponent(new PositionComponent(translate * transform))
                        .AddComponent(new PrimitivePhysicsComponent(RigidBodyType.Box));
                }
            }
        }

        private struct Hitbox
        {
            public string Name;
            public Vector3 Position;
            public Vector3 Scale;
            public Matrix Rotation;
        }

        private Hitbox[] ReadFile(string path)
        {
            List<Hitbox> boxes = new List<Hitbox>();
            string content = File.ReadAllText(Path.Combine(_world.Game.Content.RootDirectory, path));
            string[] lines = content.Split("\n");
            foreach (var x in lines)
            {
                if (x.Trim().Length != 0)
                {
                    string[] args = x.Split(':');
                    Vector3 rotation = -new Vector3(float.Parse(args[7]), float.Parse(args[9]), float.Parse(args[8]));
                    boxes.Add(new Hitbox()
                    {
                        Name = args[0],
                        Position = new Vector3(float.Parse(args[1]), float.Parse(args[3]), float.Parse(args[2])),
                        Rotation = Matrix.CreateRotationZ(MathHelper.ToRadians(rotation.Z)) *
                                    Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y)) *
                                    Matrix.CreateRotationX(MathHelper.ToRadians(-rotation.X)),
                        Scale = new Vector3(float.Parse(args[4]), float.Parse(args[6]), float.Parse(args[5])) * 2f,
                    });
                }
            }
            return boxes.ToArray();
        }

        public void DebugDraw()
        {
            for (int i = 0; i < _checkpoints.Count - 1; i++)
            {
                _world.Render.EnqueueMessage(new RenderMessageDrawLine(_checkpoints[i], _checkpoints[i+1], Color.White));
            }
        }

    }
}
