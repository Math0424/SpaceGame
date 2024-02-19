using BulletSharp;
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

        public Vector4[] Checkpoints => _checkpoints.ToArray();

        private List<Vector4> _checkpoints;
        private World _world;
        public WorldGenerationSystem(World world)
        {
            _checkpoints = new List<Vector4>();
            _world = world;
        }

        public static (byte, byte, ushort) DecodeSeed(uint value)
        {
            return ((byte)(value & byte.MaxValue), (byte)(value >> (8) & byte.MaxValue), (ushort)((ushort)(value >> (2 * 8)) & ushort.MaxValue));
        }

        public static uint CreateSeed(byte checkpoints, byte difficulty, ushort seed)
        {
            byte[] ret = new byte[32];
            BitConverter.GetBytes(checkpoints).CopyTo(ret, 0);
            BitConverter.GetBytes(difficulty).CopyTo(ret, sizeof(byte));
            BitConverter.GetBytes(seed).CopyTo(ret, sizeof(byte) + sizeof(byte));
            return BitConverter.ToUInt32(ret);
        }

        public void CreateRandomWorld(byte checkpoints, byte distanceScaling, ushort seed)
        {
            Random r = new Random(seed);
            Vector3[] randomPoints = new Vector3[checkpoints + 1];

            float difficulty = distanceScaling / (float)byte.MaxValue;
            Console.WriteLine($"Creating a world {checkpoints}:{Math.Round(difficulty, 2)}:{seed}");

            Vector3 normal = Vector3.Right;
            Vector3 currPos = Vector3.Zero;
            float distanceBetween = Math.Max(100 * difficulty, 20);
            for(int i = 1; i < checkpoints + 1; i++)
            {
                currPos += normal * distanceBetween;

                Vector3 tangent = Vector3.Normalize(Vector3.Cross(normal, Vector3.Up));
                Vector3 biTangent = Vector3.Normalize(Vector3.Cross(normal, tangent));

                float offset = 100 * difficulty;
                float randomTangent = (float)r.NextDouble() * offset;
                float randomBiTangent = (float)r.NextDouble() * offset;

                currPos += (tangent * randomTangent) + (biTangent * randomBiTangent);

                randomPoints[i] = currPos;
                normal = Vector3.Normalize(randomPoints[i] - randomPoints[i - 1]);
            }

            _checkpoints.Add(Vector4.Zero);
            Vector3[] arrowPoints = MathExtensions.CatmullRom(randomPoints, .25f);
            for (int i = 1; i < arrowPoints.Length - 1; i++)
            {
                normal = Vector3.Normalize(arrowPoints[i + 1] - arrowPoints[i - 1]);
                Vector3 pos = arrowPoints[i];

                if (i % 4 == 0)
                    SpawnCheckpoint(pos, normal);
                else
                {
                    Matrix transform = Matrix.CreateWorld(pos, normal, Vector3.Cross(pos, normal));
                    _world.CreateEntity()
                            .AddComponent(new PositionComponent(transform, Matrix.CreateScale(0.01f)))
                            .AddComponent(new MeshComponent("Models/Arrow", "Textures/Arrow/arrow_CM", "Textures/Arrow/arrow_ADD"))
                            .AddComponent(new MeshRenderingComponent() { Color = Color.Yellow })
                            .AddComponent(new ArrowAnimator());
                }
            }
        }

        private void SpawnCheckpoint(Vector3 pos, Vector3 normal)
        {
            Matrix transform = Matrix.CreateWorld(pos, normal, Vector3.Cross(pos, normal));

            _world.CreateEntity()
                .AddComponent(new PositionComponent(transform))//, Matrix.CreateScale(0.01f)))
                .AddComponent(new MeshComponent("Models/ring", "Textures/Ring/CT_ring", "Textures/Ring/ADD_ring"))
                .AddComponent(new MeshRenderingComponent());

            Hitbox[] checkpointBoxes = ReadFile("Hitboxes/Ring.txt");

            int triggerId = -1;
            foreach(var x in checkpointBoxes)
            {
                Matrix translate = Matrix.CreateScale(x.Scale) * x.Rotation * Matrix.CreateTranslation(x.Position);
                if (x.Name.ToLower() == "box")
                {
                    _world.CreateEntity()
                        .AddComponent(new PositionComponent(translate * transform))
                        .AddComponent(new PrimitivePhysicsComponent(RigidBodyType.Box));
                }
                else if(x.Name.ToLower() == "trigger")
                {
                    var trigger = _world.CreateEntity()
                        .AddComponent(new PositionComponent(translate * transform))
                        .AddComponent(new PrimitivePhysicsComponent(RigidBodyType.Box, CollisionFlags.NoContactResponse));
                    triggerId = trigger.Id;
                }
            }
            _checkpoints.Add(new Vector4(pos, triggerId));
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

    }
}
