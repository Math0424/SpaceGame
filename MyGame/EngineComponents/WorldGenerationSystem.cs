﻿using Microsoft.Xna.Framework;
using Project1.Engine;
using Project1.Engine.Components;
using Project1.Engine.Systems.RenderMessages;
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

        Vector3[] _points;
        private World _world;
        public WorldGenerationSystem(World world)
        {
            _world = world;
        }

        public void CreateRandomWorld(float difficulty)
        {
            Random r = new Random();
            int points = r.Next((int)(difficulty * 50));
            _points = new Vector3[points];

            Console.WriteLine($"Creating a world with {points} checkpoints");

            Vector3 dir = Vector3.Forward;
            Vector3 currPos = Vector3.Zero;
            float distanceBetween = 10;
            for(int i = 1; i < points; i++)
            {
                currPos += dir * distanceBetween * difficulty;
                float offset = distanceBetween / (10 * (1 - difficulty) + .1f);
                currPos += new Vector3((float)r.NextDouble() * offset * difficulty, (float)r.NextDouble() * offset * difficulty, (float)r.NextDouble() * offset * difficulty);
                
                _points[i] = currPos;

                SpawnCheckpoint(currPos, dir);

                dir = Vector3.Normalize(currPos - _points[i - 1]);
            }
        }

        private void SpawnCheckpoint(Vector3 pos, Vector3 normal)
        {
            Matrix transform = Matrix.CreateWorld(pos, normal, Vector3.Cross(pos, normal));

            _world.CreateEntity()
                .AddComponent(new PositionComponent(transform, Matrix.CreateScale(0.01f)))
                .AddComponent(new MeshComponent("Models/ring"));

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
                                    Matrix.CreateRotationX(MathHelper.ToRadians(rotation.X)),
                        Scale = new Vector3(float.Parse(args[4]), float.Parse(args[6]), float.Parse(args[5])),
                    });
                }
            }

            return boxes.ToArray();
        }

        public void DebugDraw()
        {
            for (int i = 0; i < _points.Length - 1; i++)
            {
                _world.Render.EnqueueMessage(new RenderMessageDrawLine(_points[i], _points[i+1], Color.White));
            }
        }

    }
}
